using BAMod.GlobalContent.Scripts;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;

namespace BAMod.GlobalContent.Components
{
    internal class BulletSimComponent : MonoBehaviour
    {
        public SimBulletType type;
        public bool shouldDestroy;
        public int ID;

        private float timeAlive = 0f;
        private Vector3 currentPosition;
        private Vector3 currentDirection;

        public SimBulletManager.SimBullet SimBullet;

        public int PrefabID;
        public float? arg1;
        public float? arg2;
        public float? arg3;
        public float? arg4;

        public float timeAirborne;

        void Awake()
        {
            currentPosition = transform.position;
            currentDirection = transform.forward;
            if (GetComponent<ProjectileGhostController>())
            {
                GetComponent<ProjectileGhostController>().enabled = false;
            }
        }

        void Update()
        {
            ClientVisualUpdate();
        }

        void ClientVisualUpdate()
        {
            if (shouldDestroy)
            {
                Destroy(gameObject);
                return;
            }

            timeAlive += Time.deltaTime;

            ReturnEvaluation(out var update, timeAlive);

            transform.position = update.currentPosition;
            transform.forward = update.direction;

            if (SimBullet.tracerPrefab)
            {
                var evaluation = new List<SimBulletManager.ReturnPositionalValues>();

                for (int i = 0; i < SimBullet.resolution; i++)
                {
                    float t = timeAlive * (i / (float)SimBullet.resolution);
                    float prevT = t - (timeAlive / SimBullet.resolution);
                    if (prevT < 0f) prevT = 0f;

                    ReturnEvaluation(out update, t, prevT);
                    evaluation.Add(update);
                }

                SimBulletManager.SpawnTracers(evaluation);
            }

            timeAirborne = timeAlive;
        }

        void ReturnEvaluation(out SimBulletManager.ReturnPositionalValues update, float newTime, float prevTimeOverride = -1f)
        {
            float prevTime = prevTimeOverride >= 0f ? prevTimeOverride : timeAirborne;

            switch (type)
            {
                case SimBulletType.linear:
                    SimBulletManager.LinearDrop.Evaluate(SimBullet, prevTime, newTime, out update);
                    break;

                case SimBulletType.logarithmic:
                    SimBulletManager.LogarithmicDrop.Evaluate(SimBullet, prevTime, newTime, out update);
                    break;

                case SimBulletType.exponential:
                    SimBulletManager.ExponentialDrop.Evaluate(SimBullet, prevTime, newTime, out update);
                    break;

                case SimBulletType.realisticGravity:
                    SimBulletManager.LinearDrop.Evaluate(SimBullet, prevTime, newTime, out update);
                    break;

                default:
                    SimBulletManager.LinearDrop.Evaluate(SimBullet, prevTime, newTime, out update);
                    break;
            }
        }
    }
}