using BAMod.GlobalContent.Components;
using RoR2;
using System;
using System.Collections.Generic;
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
            public List<HurtBox> hits = new List<HurtBox>();
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
        }

        public static class LinearDrop
        {
            public static void Evaluate(SimBullet bullet, float previousTime, float currentTime, out ReturnPositionalValues update)
            {
                Vector3 prevPos = bullet.origin + bullet.direction * bullet.velocity * previousTime + Vector3.down * (0.5f * bullet.dropSpeed * previousTime * previousTime);
                Vector3 currPos = bullet.origin + bullet.direction * bullet.velocity * currentTime + Vector3.down * (0.5f * bullet.dropSpeed * currentTime * currentTime);
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

                float prevDrop = bullet.dropSpeed * Mathf.Log(baseValue + prevDist * multiplier) * previousTime;
                float currDrop = bullet.dropSpeed * Mathf.Log(baseValue + currDist * multiplier) * currentTime;

                Vector3 prevPos = bullet.origin + bullet.direction * prevDist + Vector3.down * prevDrop;
                Vector3 currPos = bullet.origin + bullet.direction * currDist + Vector3.down * currDrop;
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
            public static void Evaluate(SimBullet bullet, float previousTime, float currentTime, out ReturnPositionalValues update, float baseValue = 1f, float multiplier = 1f, float power = 2f)
            {
                float prevDist = bullet.velocity * previousTime;
                float currDist = bullet.velocity * currentTime;

                float prevDrop = bullet.dropSpeed * Mathf.Pow(baseValue + prevDist * multiplier, power) * previousTime;
                float currDrop = bullet.dropSpeed * Mathf.Pow(baseValue + currDist * multiplier, power) * currentTime;

                Vector3 prevPos = bullet.origin + bullet.direction * prevDist + Vector3.down * prevDrop;
                Vector3 currPos = bullet.origin + bullet.direction * currDist + Vector3.down * currDrop;
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

        private static readonly HashSet<Collider> _hitSet = new HashSet<Collider>();
        private static readonly RaycastHit[] _hitBuffer = new RaycastHit[64];

        public static bool IsExpired(
            List<ReturnPositionalValues> points,
            SimBullet bullet,
            out RaycastHit[] validCollisions,
            out RaycastHit stopperCollision)
        {
            stopperCollision = new RaycastHit();
            _hitSet.Clear();

            int totalHits = 0;
            float totalDistance = 0f;

            foreach (var point in points)
            {
                Vector3 delta = point.currentPosition - point.previousPosition;
                float distance = delta.magnitude;

                if (distance <= 0f)
                    continue;

                Vector3 direction = delta / distance;

                if (Physics.SphereCast(
                    point.previousPosition,
                    bullet.radius,
                    direction,
                    out RaycastHit stopHit,
                    distance,
                    bullet.stopperMask,
                    QueryTriggerInteraction.Collide))
                {
                    stopperCollision = stopHit;

                    // Collect hits only up to the stopper
                    int hitCount = Physics.SphereCastNonAlloc(
                        point.previousPosition,
                        bullet.radius,
                        direction,
                        _hitBuffer,
                        stopHit.distance,
                        bullet.hitMask,
                        QueryTriggerInteraction.Collide);

                    for (int i = 0; i < hitCount; i++)
                    {
                        Collider col = _hitBuffer[i].collider;
                        if (col != null && _hitSet.Add(col))
                        {
                            _hitBuffer[totalHits++] = _hitBuffer[i];
                        }
                    }

                    validCollisions = new RaycastHit[totalHits];
                    Array.Copy(_hitBuffer, validCollisions, totalHits);
                    return true;
                }

                int count = Physics.SphereCastNonAlloc(
                    point.previousPosition,
                    bullet.radius,
                    direction,
                    _hitBuffer,
                    distance,
                    bullet.hitMask,
                    QueryTriggerInteraction.Collide);

                for (int i = 0; i < count; i++)
                {
                    Collider col = _hitBuffer[i].collider;
                    if (col != null && _hitSet.Add(col))
                    {
                        _hitBuffer[totalHits++] = _hitBuffer[i];
                    }
                }

                totalDistance += distance;

                if (totalDistance >= bullet.maximumDistance)
                {
                    break;
                }
            }

            validCollisions = new RaycastHit[totalHits];
            Array.Copy(_hitBuffer, validCollisions, totalHits);

            return false;
        }
        private static RaycastHit[] TrimResults(RaycastHit[] source, int count)
        {
            if (count == 0)
                return Array.Empty<RaycastHit>();

            RaycastHit[] result = new RaycastHit[count];
            Array.Copy(source, result, count);
            return result;
        }

        public static void Fire(SimBullet bullet)
        {
            if (bullet.aborted) return;
            bullet.fireTime = Time.time;
            var NetworkPacket = new AttemptDamagePacket()
            {
                Damage = bullet.damageInfo.damage,
                AttackerNetworkID = bullet.owner.GetComponent<NetworkIdentity>().netId,
                colorIndex = bullet.damageInfo.damageColorIndex,
                InflictorNetworkID = bullet.damageInfo.inflictor.GetComponent<NetworkIdentity>().netId,
                crit = bullet.damageInfo.crit,
                force = bullet.damageInfo.force,
                procChainMask = bullet.damageInfo.procChainMask,
                procCoefficient = bullet.damageInfo.procCoefficient,
                type = bullet.type
            };
            ServerInstance.RegisterBullet(NetworkPacket, bullet.origin, bullet.direction, bullet.velocity, bullet.dropSpeed, bullet.resolution, bullet.prefabIndex);
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

        public static void Init()
        {
            var globalSimBullet = new GameObject("SimBulletServer");

            var identity = globalSimBullet.AddComponent<NetworkIdentity>();
            var behavior = globalSimBullet.AddComponent<ServerBulletSimNetworkBehavior>();

            GameObject.DontDestroyOnLoad(globalSimBullet);

            NetworkServer.Spawn(globalSimBullet);
        }
    }
}