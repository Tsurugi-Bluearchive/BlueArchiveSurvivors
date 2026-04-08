using BAMod.GlobalContent.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace BAMod.GlobalContent.Components
{
    internal class BulletSimComponent : NetworkBehaviour
    {
        public SimBulletType type;
        public bool Destroy;
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
            if (!isClient)
            {
                Destroy(gameObject);
                Log.Warning("Client-side visual effect registered on the server! Deleting!");
                return;
            }

            currentPosition = transform.position;
            currentDirection = transform.forward;
        }

        void Update()
        {
            if (isClient)
            {
                ClientVisualUpdate();
            }
        }

        [ClientCallback]
        void ClientVisualUpdate()
        {
            if (Destroy)
            {
                Destroy(gameObject);
                return;
            }

            timeAlive += Time.deltaTime;

            ReturnEvalutation(out var update, timeAlive);

            transform.position = update.currentPosition;
            transform.forward = update.direction;

            if (SimBullet.tracerPrefab)
            {
                var evaluation = new List<SimBulletManager.ReturnPositionalValues>();

                for (int i = 0; i < SimBullet.resolution; i++)
                {
                    float t = timeAlive * (i / (float)SimBullet.resolution);
                    ReturnEvalutation(out update, t);
                    evaluation.Add(update);
                }

                SimBulletManager.SpawnTracers(evaluation);
            }

            timeAirborne = timeAlive;
        }

        void ReturnEvalutation(out SimBulletManager.ReturnPositionalValues update, float newTime)
        {
            switch (type)
            {
                case SimBulletType.linear:
                    SimBulletManager.LinearDrop.Evaluate(SimBullet, timeAirborne, newTime, out update);
                    break;

                case SimBulletType.logarithmic:
                    SimBulletManager.LogarithmicDrop.Evaluate(SimBullet, timeAirborne, newTime, out update);
                    break;

                case SimBulletType.exponential:
                    SimBulletManager.ExponentialDrop.Evaluate(SimBullet, timeAirborne, newTime, out update);
                    break;

                case SimBulletType.realisticGravity:
                    SimBulletManager.LinearDrop.Evaluate(SimBullet, timeAirborne, newTime, out update);
                    break;

                default:
                    SimBulletManager.LinearDrop.Evaluate(SimBullet, timeAirborne, newTime, out update);
                    break;
            }
        }
        void OnDestroy()
        {
            if (!isClient) return;
        }
    }
}