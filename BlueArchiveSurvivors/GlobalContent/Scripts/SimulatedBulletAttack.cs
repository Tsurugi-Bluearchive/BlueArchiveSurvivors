using BAMod.GlobalContent.Components;
using IL.RoR2.Items;
using RoR2;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Transactions;
using UnityEngine;
using static BAMod.GlobalContent.Scripts.SimulatedBulletAttack;

namespace BAMod.GlobalContent.Scripts
{
    static class SimulatedBulletAttack
    {
        public static BulletSimNetworkBehavior ServerInstance;
        public static Dictionary<int, ClientSimBullet> ClientBullets;
        public static int nextSimBulletInstance;
        public static GameObject networkController;
        public class ReturnPositionalValues
        {
            public Vector3 previousPosition;
            public Vector3 currentPosition;
            public Vector3 direction;
            public float distanceTraveled;
        }

        public interface ISimBulletAttack
        {
            public void Evaluate(out ReturnPositionalValues update, float previousTime, float currentTime);
            public bool HasResolution(out uint resolution);
            public Type ReturnAttackType();
        }

        public class ClientSimBullet
        {
            public GameObject tracerPrefab;
            public GameObject ghostPrefab;
            public ReturnPositionalValues position;
        }
        public class SimBullet
        {
            public SimBullet(SimBulletType type, float? arg1 = null, float? arg2 = null, float? arg3 = null)
            {
                switch (type)
                {
                    case SimBulletType.exponential:
                        attack = new ExponentialDropAttack()
                        {
                            bullet = this,
                            exponentialBaseValue = arg1 ?? 1f,
                            exponentialMultiplier = arg2 ?? 1f,
                            exponentialPower = arg3 ?? 2f
                        };
                        break;

                    case SimBulletType.linear:
                        attack = new LinearDropAttack()
                        {
                            bullet = this
                        };
                        break;

                    case SimBulletType.logarithmic:
                        attack = new LogarithmicDropAttack()
                        {
                            bullet = this,
                            logarithmicBaseValue = arg1 ?? 1f,
                            logarithmicMultiplier = arg2 ?? 0.02f
                        };
                        break;

                    case SimBulletType.realisticGravity:
                        // TODO: Implement RealisticGravityAttack later
                        attack = null;
                        break;

                    default:
                        attack = new LinearDropAttack() { bullet = this };
                        break;
                }
            }

            // ─────────────────────────────────────────────────────────────
            // Fields
            // ─────────────────────────────────────────────────────────────
            public bool aborted;
            public GameObject tracerPrefab;
            public GameObject ghostPrefab;
            public GameObject owner;
            public DamageInfo damageInfo;
            public List<HurtBox> hits = new List<HurtBox>();

            public ISimBulletAttack attack;
            public Vector3 origin;
            public Vector3 direction;
            public LayerMask hitMask;
            public LayerMask stopperMask;
            public float radius = 0.5f;
            public float maximumDistance = 1000f;
            public float velocity;
            public float dropSpeed;
            public byte resolution = 8;
            public float travelTime;
            public float fireTime;
        }

        public class LinearDropAttack : ISimBulletAttack
        {
            public SimBullet bullet;

            public void Evaluate(out ReturnPositionalValues update, float previousTime, float currentTime)
            {
                float previousTimeValue = previousTime;
                float currentTimeValue = currentTime;

                Vector3 previousPosition =
                    bullet.origin +
                    bullet.direction * bullet.velocity * previousTimeValue +
                    Vector3.down * (0.5f * bullet.dropSpeed * previousTimeValue * previousTimeValue);

                Vector3 currentPosition =
                    bullet.origin +
                    bullet.direction * bullet.velocity * currentTimeValue +
                    Vector3.down * (0.5f * bullet.dropSpeed * currentTimeValue * currentTimeValue);

                Vector3 direction = (currentPosition - previousPosition).normalized;

                update = new ReturnPositionalValues()
                {
                    previousPosition = previousPosition,
                    currentPosition = currentPosition,
                    direction = direction,
                    distanceTraveled = Vector3.Distance(previousPosition, currentPosition)
                };
            }

            public bool HasResolution(out uint resolution)
            {
                resolution = bullet.resolution;
                return resolution != 0;
            }

            public Type ReturnAttackType()
            {
                return typeof(LinearDropAttack);
            }
        }

        public class LogarithmicDropAttack : ISimBulletAttack
        {
            public SimBullet bullet;
            public float logarithmicBaseValue = 1f;
            public float logarithmicMultiplier = 0.02f;

            public void Evaluate(out ReturnPositionalValues update, float previousTime, float currentTime)
            {
                float previousTimeValue = previousTime;
                float currentTimeValue = currentTime;

                float previousDistance = bullet.velocity * previousTimeValue;
                float currentDistance = bullet.velocity * currentTimeValue;

                float previousDrop =
                    bullet.dropSpeed *
                    Mathf.Log(logarithmicBaseValue + previousDistance * logarithmicMultiplier) *
                    previousTimeValue;

                float currentDrop =
                    bullet.dropSpeed *
                    Mathf.Log(logarithmicBaseValue + currentDistance * logarithmicMultiplier) *
                    currentTimeValue;

                Vector3 previousPosition =
                    bullet.origin +
                    bullet.direction * previousDistance +
                    Vector3.down * previousDrop;

                Vector3 currentPosition =
                    bullet.origin +
                    bullet.direction * currentDistance +
                    Vector3.down * currentDrop;

                Vector3 direction = (currentPosition - previousPosition).normalized;

                update = new ReturnPositionalValues()
                {
                    previousPosition = previousPosition,
                    currentPosition = currentPosition,
                    direction = direction,
                    distanceTraveled = Vector3.Distance(previousPosition, currentPosition)
                };
            }

            public bool HasResolution(out uint resolution)
            {
                resolution = bullet.resolution;
                return resolution != 0;
            }

            public Type ReturnAttackType()
            {
                return typeof(LogarithmicDropAttack);
            }
        }

        public class ExponentialDropAttack : ISimBulletAttack
        {
            public SimBullet bullet;
            public float exponentialBaseValue = 1f;
            public float exponentialMultiplier = 1f;
            public float exponentialPower = 2f;

            public void Evaluate(out ReturnPositionalValues update, float previousTime, float currentTime)
            {
                float previousTimeValue = previousTime;
                float currentTimeValue = currentTime;

                float previousDistance = bullet.velocity * previousTimeValue;
                float currentDistance = bullet.velocity * currentTimeValue;

                float previousDrop =
                    bullet.dropSpeed *
                    Mathf.Pow(exponentialBaseValue + previousDistance * exponentialMultiplier, exponentialPower) *
                    previousTimeValue;

                float currentDrop =
                    bullet.dropSpeed *
                    Mathf.Pow(exponentialBaseValue + currentDistance * exponentialMultiplier, exponentialPower) *
                    currentTimeValue;

                Vector3 previousPosition =
                    bullet.origin +
                    bullet.direction * previousDistance +
                    Vector3.down * previousDrop;

                Vector3 currentPosition =
                    bullet.origin +
                    bullet.direction * currentDistance +
                    Vector3.down * currentDrop;

                Vector3 direction = (currentPosition - previousPosition).normalized;

                update = new ReturnPositionalValues()
                {
                    previousPosition = previousPosition,
                    currentPosition = currentPosition,
                    direction = direction,
                    distanceTraveled = Vector3.Distance(previousPosition, currentPosition)
                };
            }

            public bool HasResolution(out uint resolution)
            {
                resolution = bullet.resolution;
                return resolution != 0;
            }

            public Type ReturnAttackType()
            {
                return typeof(ExponentialDropAttack);
            }
        }

        public class RealisticGravityAttack
        {

        }

        public static void SpawnTracers(List<ReturnPositionalValues> points)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {

            }
        }

        public static bool isExpired(List<ReturnPositionalValues> points, SimBullet bullet, out RaycastHit[] validCollisions, out RaycastHit stopperCollision)
        {
            var hitList = new List<RaycastHit>();

            foreach (var point in points)
            {
                if (Physics.SphereCast(point.previousPosition, bullet.radius, point.direction, out stopperCollision, point.distanceTraveled, bullet.stopperMask, QueryTriggerInteraction.Collide))
                {
                    foreach (var hitThing in Physics.SphereCastAll(point.previousPosition, bullet.radius, point.direction, stopperCollision.distance, bullet.hitMask))
                    {
                        hitList.Add(hitThing);
                    }
                    validCollisions = hitList.ToArray();
                    return true;
                }
                else
                {
                    foreach (var hitThing in Physics.SphereCastAll(point.previousPosition, bullet.radius, point.direction, point.distanceTraveled, bullet.hitMask))
                    {
                        hitList.Add(hitThing);
                    }
                }
            }
            stopperCollision = new RaycastHit();
            validCollisions = hitList.ToArray();
            return false;
        }
        public static void Fire(SimBullet bullet, ISimBulletAttack attackType, DamageInfo damageInfo)
        {
            if (bullet.aborted) return;

            bullet.fireTime = Time.time;

        }

        public static void Init()
        {

        }

    }
}