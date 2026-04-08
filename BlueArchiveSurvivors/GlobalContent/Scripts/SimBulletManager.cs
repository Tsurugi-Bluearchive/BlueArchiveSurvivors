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

        public static Dictionary<int, (BulletSimComponent bulletInstance, SimBullet bullet)> ClientBullets = new();
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
            public GameObject ghostPrefab;
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

        public static bool IsExpired(List<ReturnPositionalValues> points, SimBullet bullet, out RaycastHit[] validCollisions, out RaycastHit stopperCollision)
        {
            var hitList = new List<RaycastHit>();

            foreach (var point in points)
            {
                if (Physics.SphereCast(point.previousPosition, bullet.radius, point.direction, out stopperCollision, point.distanceTraveled, bullet.stopperMask, QueryTriggerInteraction.Collide))
                {
                    foreach (var hitThing in Physics.SphereCastAll(point.previousPosition, bullet.radius, point.direction, stopperCollision.distance, bullet.hitMask))
                        hitList.Add(hitThing);

                    validCollisions = hitList.ToArray();
                    return true;
                }
                else
                {
                    foreach (var hitThing in Physics.SphereCastAll(point.previousPosition, bullet.radius, point.direction, point.distanceTraveled, bullet.hitMask))
                        hitList.Add(hitThing);
                }
            }

            stopperCollision = new RaycastHit();
            validCollisions = hitList.ToArray();
            return false;
        }

        public static void Fire(SimBullet bullet)
        {
            if (bullet.aborted) return;
            bullet.fireTime = Time.time;
        }

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

    }
}