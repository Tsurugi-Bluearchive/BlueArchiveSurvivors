using RoR2.Projectile;
using KinematicCharacterController;
using Rewired.ComponentControls.Data;
using RoR2;
using RoR2.SolusWeb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SocialPlatforms;

namespace BAMod.GlobalContent.AttackComponents
{
    internal class ArcBulletAttack : MonoBehaviour
    {
        public GameObject owner { get; set; }
        public CharacterBody ownerCharacterBody { get; set; }
        public TeamComponent ownerTeamComponent { get; set; }
        public DamageTypeCombo damageType { get; set; }
        public float damage { get; set; }
        public float procCoefficient { get; set; }
        public bool isCrit { get; set; }

        public bool canRejectForce { get; set; }
        public Vector3 force { get; set; }
        public PhysForceFlags physForceFlags { get; set; }
        public ProcChainMask procChainMask { get; set; }
        public DamageColorIndex damageColorIndex { get; set; }

        /// <summary>
        /// Additional argument values for the equation, see the enum values for their documentation
        /// </summary>
        public float[] equationArgs { get; set; }

        /// <summary>
        /// The arch type of an arc bullet attack
        /// </summary>
        public enum ArchType
        {
            /// <summary>
            /// Arch in a logarithmic trajectory <br/>
            /// equationArgs[0] + Dowwards Velocity * equationArgs[1]
            /// </summary>
            Logarithm,
            /// <summary>
            /// Arch in an exponentionally increasing arch downwards from the start to the impact <br/>
            /// (equationArgs[0] + Dowwards Velocity * equationArgs[1], equationArgs[2])
            /// </summary>
            Exponent,
            /// <summary>
            /// Linearially arch down at a consistent speed <br/>
            /// equationArgs[0] + distanceTraveled * equationArgs[1]
            /// </summary>
            Linear,
            /// <summary>
            /// Realistic gravity calculation <br/>
            /// Velocity in meters a second squared squared
            /// </summary>
            AdvGravity
        }

        /// <summary>
        /// How many capsule casts it should do per simulation step.
        /// </summary>
        public uint resolution = 10U;

        public GameObject muzzle { get; set; }

        public int muzzleIndex { get; set; }

        /// <summary>
        /// The type of arch we should be using
        /// </summary>
        public ArchType Arch = ArchType.Logarithm;

        private float downwardsVelocity = 0;
        /// <summary>
        /// The projectile ghost if you're using one. <br/>
        /// Let it remain null if you're not using one
        /// </summary>
        public GameObject projectileGhostPrefab = null;

        private ProjectileGhostController projectileGhostController;

        /// <summary>
        /// The tracer you're attempting to use for the arc <br/>
        /// Let it remain null if you aren't using one
        /// </summary>
        public GameObject tracerEffectPrefab = null;

        /// <summary>
        /// The speed in units/second it should take to travel along the parabola ,br/>
        /// Keep null if it's supposed to be instant.<br/>
        /// Negative values are supported (And will invert the attack from the point of strike towards the player)
        /// </summary>
        public float? speed = null;

        public float timeAirborne { get; private set; }

        /// <summary>
        /// The maxixmum distance at which the simulation stops
        /// </summary>
        public float maxDistance = 400f;

        /// <summary>
        /// The redius of the capsuleCasts
        /// </summary>
        public float radius = 1f;

        public Vector3 origin { get; set; }

        public Vector3 direction { get; set; }
        /// <summary>
        /// What it will consider the end of the parabola
        /// </summary>
        public LayerMask stopperMask { get; set; }

        /// <summary>
        /// The hit mask for the capsule casts
        /// </summary>
        public LayerMask HitMask { get; set; }

        /// <summary>
        /// The speed in units per second that it will drop, affected by the equation
        /// </summary>
        public float dropSpeed { get; set; } = 5f;

        private List<HealthComponent> attackedHealthComponents = new();

        private bool simulated = false;

        public float timeTraveledFor { get; private set; } = 0f;

        private bool fired = false;
        /// <summary>
        /// The distance traveled by le bullet
        /// </summary>
        public float distanceTraveled { get; private set; } = 0f;

        public Vector3 projectilePosition { get; private set; } = Vector3.zero;
        public List<RaycastHit> bulletHits { get; private set; } = new();
        public RaycastHit hitpoint { get; private set; }

        protected static EffectData pooledEffectData = new EffectData();
        public void Fire()
        {
            if (speed != null)
            {
                if (speed != 0)
                {
                    simulated = true;
                }
                else
                {
                    Log.Error("ArchBulletAttack cannot have a speed of 0!");
                }
            }
            StartCoroutine(Simulator());
        }
        private IEnumerator Simulator()
        {
            while(Simulate(Time.fixedDeltaTime, projectilePosition, out var hits, out var stopper, out var points))
            {
                DamageForeach(hits);
                if (tracerEffectPrefab)
                {
                    foreach (var point in points)
                    {
                        InstantiateTracer(point.start, point.end, owner, muzzleIndex);
                    }
                }
                if (projectileGhostPrefab)
                {
                    UpdateProjectileGhost(Time.fixedDeltaTime, points.Last().end, points.Last().direction);
                }
                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        public bool InstantiateTracer(Vector3 end, Vector3 start, GameObject weapon, int muzzleIndex)
        {
            if (tracerEffectPrefab == null)
                return false;
            pooledEffectData.origin = end;
            pooledEffectData.start = start;
            pooledEffectData.SetChildLocatorTransformReference(weapon, muzzleIndex);
            EffectManager.SpawnEffect(tracerEffectPrefab, pooledEffectData, transmit: true);
            pooledEffectData.Reset();
            return true;
        }

        public bool UpdateProjectileGhost(float timeScale, Vector3 simulationPoint, Vector3 direction)
        {
            if (projectileGhostPrefab == null)
                return false;

            if (!projectileGhostController)
            {
                projectileGhostController = Instantiate(projectileGhostPrefab)
                    .GetComponent<ProjectileGhostController>();
            }

            Transform transform = projectileGhostController.transform;

            transform.position = simulationPoint;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            projectileGhostController.authorityTransform = transform;

            return true;
        }
        private void DamageForeach(RaycastHit[] hits)
        {
            foreach (var hit in hits)
            {
                if (ConstructDamageInfo(hit, out var damageInfo, out var healthComponent))
                {
                    if (!attackedHealthComponents.Contains(healthComponent))
                        healthComponent.TakeDamage(damageInfo);
                }
            }
        }
        private bool ConstructDamageInfo(RaycastHit hit, out DamageInfo constructedDamageInfo, out HealthComponent healthComponent)
        {
            if (hit.collider != null &&
                hit.collider.TryGetComponent<HurtBox>(out var hurtBox) &&
                hurtBox.healthComponent != null &&
                hurtBox.healthComponent.body != null)
            {
                constructedDamageInfo = new DamageInfo()
                {
                    damage = damage,
                    damageType = damageType,
                    attacker = owner,
                    inflictor = hurtBox.gameObject,
                    inflictedHurtbox = hurtBox,
                    crit = isCrit,
                    canRejectForce = canRejectForce,
                    force = force,
                    procChainMask = procChainMask,
                    physForceFlags = physForceFlags,
                    procCoefficient = procCoefficient,
                    position = hit.point,
                    damageColorIndex = damageColorIndex,
                };
                healthComponent = hurtBox.healthComponent;
                return true;
            }
            healthComponent = null;
            constructedDamageInfo = null;
            return false;
        }

        private bool Simulate(float timeScale, Vector3 start, out RaycastHit[] validHits, out RaycastHit? stopperHit, out List<(float distance, Vector3 direction, Vector3 start, Vector3 end)> simulatedPoints)
        {
            simulatedPoints = new List<(float distance, Vector3 direction, Vector3 start, Vector3 end)>();
            var currentPoint = start;
            var validHitsList = new List<RaycastHit>();
            RaycastHit? stopper = null;
            for (uint i = 0; i < resolution; i++)
            {
                switch (Arch)
                {
                    case ArchType.Logarithm:
                        Logarithm(out var segment, timeScale);
                        simulatedPoints.Add((distanceTraveled, segment.segmentDirection, segment.segmentEndPosition, segment.segmentStartPosition));
                        currentPoint = segment.segmentEndPosition;
                        break;
                    case ArchType.Exponent:
                        Exponent(out segment, timeScale);
                        simulatedPoints.Add((distanceTraveled, segment.segmentDirection, segment.segmentEndPosition, segment.segmentStartPosition));
                        currentPoint = segment.segmentEndPosition;
                        break;
                    case ArchType.Linear:
                        Linear(out segment, timeScale);
                        simulatedPoints.Add((distanceTraveled, segment.segmentDirection, segment.segmentEndPosition, segment.segmentStartPosition));
                        currentPoint = segment.segmentEndPosition;
                        break;
                    case ArchType.AdvGravity:
                        Log.Error("Advanced Gravity is not implemented yet!");
                        break;
                }
            }
            foreach (var segment in simulatedPoints)
            {
                if(!IsExpired(segment.start, segment.direction, segment.distance, out var hits, out var stopperstopper))
                {
                    foreach (var hit in hits)
                    {
                        validHitsList.Add(hit);
                    }
                    stopper = stopperstopper;
                }
                else
                {
                    validHits = null;
                    stopperHit = null;
                    return false;
                }
            }
            stopperHit = stopper;
            validHits = validHitsList.ToArray();
            return true;
        }

        public bool IsExpired(Vector3 start, Vector3 direction, float segmentLength, out RaycastHit[] validHits, out RaycastHit stopperhit)
        {
            if (segmentLength <= 0f || direction.sqrMagnitude < 0.0001f)
            {
                validHits = Array.Empty<RaycastHit>();
                stopperhit = default;
                return true;
            }

            direction = direction.normalized;

            float projectedDistanceTraveled = distanceTraveled + segmentLength;

            if (projectedDistanceTraveled >= maxDistance)
            {
                float remainingDistance = maxDistance - distanceTraveled;
                if (remainingDistance <= 0f)
                {
                    validHits = Array.Empty<RaycastHit>();
                    stopperhit = default;
                    return true;
                }

                bool hitStopper = Physics.CapsuleCast(start, start + direction * remainingDistance, radius,
                    direction, out stopperhit, remainingDistance, stopperMask, QueryTriggerInteraction.Collide);

                if (hitStopper)
                {
                    validHits = Physics.CapsuleCastAll(start, start + direction * remainingDistance, radius,
                        direction, stopperhit.distance, HitMask, QueryTriggerInteraction.Collide);
                }
                else
                {
                    validHits = Physics.CapsuleCastAll(start, start + direction * remainingDistance, radius,
                        direction, remainingDistance, HitMask, QueryTriggerInteraction.Collide);
                }
                return true;
            }

            bool collided = Physics.CapsuleCast(start, start + direction * segmentLength, radius,
                direction, out stopperhit, segmentLength, stopperMask, QueryTriggerInteraction.Collide);

            if (collided)
            {
                validHits = Physics.CapsuleCastAll(start, start + direction * segmentLength, radius,
                    direction, stopperhit.distance, HitMask, QueryTriggerInteraction.Collide);
                return true; // Hit wall/ground → stop simulation
            }
            else
            {
                validHits = Physics.CapsuleCastAll(start, start + direction * segmentLength, radius,
                    direction, segmentLength, HitMask, QueryTriggerInteraction.Collide);
                return false; // Continue
            }
        }

        private void Logarithm(out (Vector3 segmentEndPosition, Vector3 segmentStartPosition, Vector3 segmentDirection) calculations, float timeScale)
        {
            var calculatedBulletPositionStart = origin + direction * distanceTraveled + Vector3.down * downwardsVelocity;
            downwardsVelocity += (dropSpeed * Mathf.Log(equationArgs[0] + distanceTraveled * equationArgs[1]) * timeScale) / resolution;
            distanceTraveled += ((float)speed * timeScale) / resolution;
            var calculatedBulletPositionEnd = origin + direction * distanceTraveled + Vector3.down * downwardsVelocity;
            var segmentDirection = (calculatedBulletPositionEnd - calculatedBulletPositionStart).normalized;
            calculations = (calculatedBulletPositionEnd, calculatedBulletPositionStart, segmentDirection);
        }

        private void Exponent(out (Vector3 segmentEndPosition, Vector3 segmentStartPosition, Vector3 segmentDirection) calculations, float timeScale)
        {
            var calculatedBulletPositionStart = origin + direction * distanceTraveled + Vector3.down * downwardsVelocity;
            downwardsVelocity += (dropSpeed * Mathf.Pow(equationArgs[0] + distanceTraveled * equationArgs[1], equationArgs[2]) * timeScale) / resolution;
            distanceTraveled += ((float)speed * timeScale) / resolution;
            var calculatedBulletPositionEnd = origin + direction * distanceTraveled + Vector3.down * downwardsVelocity;
            var segmentDirection = (calculatedBulletPositionEnd - calculatedBulletPositionStart).normalized;
            calculations = (calculatedBulletPositionEnd, calculatedBulletPositionStart, segmentDirection);
        }

        private void Linear(out (Vector3 segmentEndPosition, Vector3 segmentStartPosition, Vector3 segmentDirection) calculations, float timeScale)
        {
            var calculatedBulletPositionStart = origin + direction * distanceTraveled + Vector3.down * downwardsVelocity;
            downwardsVelocity += (dropSpeed * (equationArgs[0] + distanceTraveled * equationArgs[1]) * timeScale) / resolution;
            distanceTraveled += ((float)speed * timeScale) / resolution;
            var calculatedBulletPositionEnd = origin + direction * distanceTraveled + Vector3.down * downwardsVelocity;
            var segmentDirection = (calculatedBulletPositionEnd - calculatedBulletPositionStart).normalized;
            calculations = (calculatedBulletPositionEnd, calculatedBulletPositionStart, segmentDirection);
        }
    }
}
