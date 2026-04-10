using BAMod.GlobalContent.Components;
using BAMod.Mashiro.Content;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using RoR2BepInExPack.GameAssetPaths;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Networking;

namespace BAMod.GlobalContent.Scripts
{
    static class SimBulletManager
    {
        public static ServerBulletSimNetworkBehavior ServerInstance;
        public static int nextSimBulletInstance;
        public static GameObject networkController;
        private static List<GameObject> SimBulletPrefabs = new();
        public class ReturnPositionalValues
        {
            public Vector3 previousPosition;
            public Vector3 currentPosition;
            public Vector3 direction;
            public float distanceTraveled;
        }

        public class SimBullet
        {
            public bool aborted;
            public SimBulletType type;
            public GameObject tracerPrefab;
            public GameObject owner;
            public DamageInfo damageInfo;
            public List<HealthComponent> hitHealthComponents = new List<HealthComponent>();
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
            public GameObject simBulletPrefab;
            public bool active;
            public int prefabIndex;

            // New explosion fields
            public bool explodeOnPassthrough;
            public bool explodeOnExpire;
            public float explosionRadius = 8f;
            public float explosionDamage = 1f;
            public DamageTypeCombo explosionDamageType = DamageTypeCombo.Generic;
            public float explosionProcCoefficient = 1f;
            public float explosionForce = 0f;
            public BlastAttack.FalloffModel falloffModel;

            public SimBullet Clone()
            {
                return new SimBullet
                {
                    aborted = this.aborted,
                    type = this.type,
                    tracerPrefab = this.tracerPrefab,
                    owner = this.owner,
                    damageInfo = this.damageInfo,
                    hitHealthComponents = new List<HealthComponent>(this.hitHealthComponents),
                    origin = this.origin,
                    direction = this.direction,
                    hitMask = this.hitMask,
                    stopperMask = this.stopperMask,
                    radius = this.radius,
                    maximumDistance = this.maximumDistance,
                    velocity = this.velocity,
                    dropSpeed = this.dropSpeed,
                    resolution = this.resolution,
                    travelTime = this.travelTime,
                    fireTime = this.fireTime,
                    simBulletPrefab = this.simBulletPrefab,
                    active = this.active,
                    prefabIndex = this.prefabIndex,

                    // Explosion fields
                    explodeOnPassthrough = this.explodeOnPassthrough,
                    explodeOnExpire = this.explodeOnExpire,
                    explosionRadius = this.explosionRadius,
                    explosionDamage = this.explosionDamage,
                    explosionDamageType = this.explosionDamageType,
                    explosionProcCoefficient = this.explosionProcCoefficient,
                    explosionForce = this.explosionForce

                };
            }
            public void Explode(Vector3 position, Vector3? impactNormal = null)
            {
                if (owner == null || damageInfo?.attacker == null)
                {
                    Debug.LogWarning("SimBullet.Explode: Missing owner or attacker!");
                    return;
                }

                BlastAttack blast = new BlastAttack
                {
                    position = position,
                    radius = explosionRadius,
                    baseDamage = damageInfo.damage * explosionDamage,
                    baseForce = explosionForce,
                    damageType = explosionDamageType,
                    procCoefficient = explosionProcCoefficient,
                    attacker = damageInfo.attacker,
                    inflictor = owner,
                    teamIndex = damageInfo.attacker.GetComponent<TeamComponent>()?.teamIndex ?? TeamIndex.None,
                    falloffModel = falloffModel
                };

                if (impactNormal.HasValue)
                {
                    blast.position += impactNormal.Value * 0.5f;
                }

                BlastAttack.Result result = blast.Fire();
            }
        }
        public static class LinearDrop
        {
            public static void Evaluate(SimBullet bullet, float previousTime, float currentTime, out ReturnPositionalValues update)
            {
                float g = bullet.dropSpeed;

                Vector3 prevPos = bullet.origin
                    + bullet.direction * bullet.velocity * previousTime
                    + Vector3.down * (0.5f * g * previousTime * previousTime);

                Vector3 currPos = bullet.origin
                    + bullet.direction * bullet.velocity * currentTime
                    + Vector3.down * (0.5f * g * currentTime * currentTime);

                Vector3 dir = (currPos - prevPos).normalized;

                update = new ReturnPositionalValues()
                {
                    previousPosition = prevPos,
                    currentPosition = currPos,
                    direction = dir,
                    distanceTraveled = Vector3.Distance(prevPos, currPos)
                };
            }
        }

        public static class LogarithmicDrop
        {
            public static void Evaluate(SimBullet bullet, float previousTime, float currentTime, out ReturnPositionalValues update, float baseValue = 1f, float multiplier = 0.02f)
            {
                float prevDist = bullet.velocity * previousTime;
                float currDist = bullet.velocity * currentTime;

                float gPrev = bullet.dropSpeed * Mathf.Log(baseValue + prevDist * multiplier);
                float gCurr = bullet.dropSpeed * Mathf.Log(baseValue + currDist * multiplier);

                Vector3 prevPos = bullet.origin
                    + bullet.direction * prevDist
                    + Vector3.down * (0.5f * gPrev * previousTime * previousTime);

                Vector3 currPos = bullet.origin
                    + bullet.direction * currDist
                    + Vector3.down * (0.5f * gCurr * currentTime * currentTime);

                Vector3 dir = (currPos - prevPos).normalized;

                update = new ReturnPositionalValues()
                {
                    previousPosition = prevPos,
                    currentPosition = currPos,
                    direction = dir,
                    distanceTraveled = Vector3.Distance(prevPos, currPos)
                };
            }
        }

        public static class ExponentialDrop
        {
            public static void Evaluate(SimBullet bullet, float previousTime, float currentTime, out ReturnPositionalValues update, float baseValue = 1f, float multiplier = 0.01f, float power = 2f)
            {
                float prevDist = bullet.velocity * previousTime;
                float currDist = bullet.velocity * currentTime;

                float gPrev = bullet.dropSpeed * Mathf.Pow(baseValue + prevDist * multiplier, power);
                float gCurr = bullet.dropSpeed * Mathf.Pow(baseValue + currDist * multiplier, power);

                Vector3 prevPos = bullet.origin
                    + bullet.direction * prevDist
                    + Vector3.down * (0.5f * gPrev * previousTime * previousTime);

                Vector3 currPos = bullet.origin
                    + bullet.direction * currDist
                    + Vector3.down * (0.5f * gCurr * currentTime * currentTime);

                Vector3 dir = (currPos - prevPos).normalized;

                update = new ReturnPositionalValues()
                {
                    previousPosition = prevPos,
                    currentPosition = currPos,
                    direction = dir,
                    distanceTraveled = Vector3.Distance(prevPos, currPos)
                };
            }
        }

        public static bool IsExpired(
            List<ReturnPositionalValues> points,
            SimBullet bullet,
            out RaycastHit[] validCollisions,
            out RaycastHit stopperCollision)
        {
            stopperCollision = new RaycastHit();
            validCollisions = Array.Empty<RaycastHit>();

            if (points == null || points.Count < 2)
                return false;

            int segmentCount = points.Count - 1;

            var commands = new NativeArray<SpherecastCommand>(segmentCount, Allocator.TempJob);
            var stopperHits = new NativeArray<RaycastHit>(segmentCount, Allocator.TempJob);

            for (int i = 0; i < segmentCount; i++)
            {
                Vector3 start = points[i].currentPosition;
                Vector3 end = points[i + 1].currentPosition;
                Vector3 dir = (end - start).normalized;
                float dist = Vector3.Distance(start, end);

                commands[i] = new SpherecastCommand(
                    start,
                    bullet.radius,
                    dir,
                    dist,
                    bullet.stopperMask
                );
            }

            JobHandle handle = SpherecastCommand.ScheduleBatch(commands, stopperHits, 32);
            handle.Complete();

            float closestDistanceAlongPath = float.PositiveInfinity;
            float accumulated = 0f;

            for (int i = 0; i < segmentCount; i++)
            {
                var hit = stopperHits[i];
                if (hit.collider != null)
                {
                    float distToHit = accumulated + hit.distance;
                    if (distToHit < closestDistanceAlongPath)
                    {
                        closestDistanceAlongPath = distToHit;
                        stopperCollision = hit;
                    }
                }
                accumulated += Vector3.Distance(points[i].currentPosition, points[i + 1].currentPosition);
            }

            float evaluatedMaxDistance = stopperCollision.collider != null
                ? closestDistanceAlongPath
                : bullet.maximumDistance;

            var hitCommands = new NativeArray<SpherecastCommand>(segmentCount, Allocator.TempJob);
            int validCommandCount = 0;
            float currentDistance = 0f;

            for (int i = 0; i < segmentCount; i++)
            {
                Vector3 start = points[i].currentPosition;
                Vector3 end = points[i + 1].currentPosition;
                Vector3 dir = (end - start).normalized;
                float segmentLength = Vector3.Distance(start, end);

                float remaining = evaluatedMaxDistance - currentDistance;
                if (remaining <= 0f) break;

                float castDistance = Mathf.Min(segmentLength, remaining);

                hitCommands[validCommandCount] = new SpherecastCommand(
                    start,
                    bullet.radius,
                    dir,
                    castDistance,
                    bullet.hitMask
                );

                currentDistance += castDistance;
                validCommandCount++;

                if (castDistance < segmentLength) break;
            }

            if (validCommandCount == 0)
                return stopperCollision.collider != null;

            using var hits = new NativeArray<RaycastHit>(validCommandCount, Allocator.TempJob);

            handle = SpherecastCommand.ScheduleBatch(
                hitCommands.GetSubArray(0, validCommandCount),
                hits,
                32
            );
            handle.Complete();

            validCollisions = new RaycastHit[validCommandCount];
            hits.CopyTo(validCollisions);

            return stopperCollision.collider != null;
        }

        public static void Fire(SimBullet bullet)
        {
            if (bullet.aborted) return;
            if (ServerInstance.gameObject.activeSelf == false)
            {
                ServerInstance.gameObject.SetActive(true);

            }
            bullet.fireTime = Time.time;
            var NetworkPacket = new AttemptDamagePacket()
            {
                Damage = bullet.damageInfo.damage,
                AttackerNetworkID = bullet.owner.GetComponent<NetworkIdentity>().netId,
                colorIndex = bullet.damageInfo.damageColorIndex,
                InflictorNetworkID = bullet.damageInfo.inflictor ? bullet.damageInfo.inflictor.GetComponent<NetworkIdentity>().netId : bullet.owner.GetComponent<NetworkIdentity>().netId,
                crit = bullet.damageInfo.crit,
                force = bullet.damageInfo.force,
                procChainMask = bullet.damageInfo.procChainMask,
                procCoefficient = bullet.damageInfo.procCoefficient,
                type = bullet.type
            };
            ServerInstance.RegisterBullet(NetworkPacket, bullet.origin, bullet.direction, bullet.velocity, bullet.dropSpeed, bullet.resolution, bullet.prefabIndex, bullet.hitMask, bullet.stopperMask, bullet.radius, MashiroAssets.MashiroSmallBullet);
        }

        /// <summary>
        /// Not yet implemented
        /// </summary>
        /// <param name="points"></param>
        public static void SpawnTracers(List<ReturnPositionalValues> points)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
            }
        }

        public static void RegisterSimBulletObject(out int Index, GameObject bulletObject)
        {
            if (!bulletObject.GetComponent<BulletSimComponent>())
            {
                bulletObject.AddComponent<BulletSimComponent>();
            }
            if (!bulletObject.GetComponent<NetworkIdentity>())
            {
                bulletObject.AddComponent<NetworkIdentity>();
            }
            if (bulletObject.GetComponent<ProjectileGhostController>())
            {
                bulletObject.GetComponent<ProjectileGhostController>().enabled = false;
            }
            SimBulletPrefabs.Add(bulletObject);
            Index = SimBulletPrefabs.Count - 1;
        }

        public static bool ReturnSimbBulletObject(out GameObject item, int index)
        {
            item = null;

            if (index >= 0 && index < SimBulletPrefabs.Count)
            {
                item = SimBulletPrefabs[index];
                return true;
            }

            Log.Error("Please Select a valid SimBulletIndex");
            return false;
        }

        public static void Init(bool client = false)
        {
            var globalSimBullet = GameObject.Instantiate(new GameObject("SimBulletSever"));

            var identity = globalSimBullet.AddComponent<NetworkIdentity>();
            var behavior = globalSimBullet.AddComponent<ServerBulletSimNetworkBehavior>();
            
            NetworkServer.Spawn(globalSimBullet);
        }
    }
}