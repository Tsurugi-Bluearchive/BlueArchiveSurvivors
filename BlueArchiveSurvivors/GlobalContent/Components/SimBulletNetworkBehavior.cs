using BAMod.GlobalContent.Scripts;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static BAMod.GlobalContent.Scripts.SimBulletManager;

namespace BAMod.GlobalContent.Components
{
    internal class SimBulletNetworkBehavior : NetworkBehaviour
    {
        public SimBulletType type;
        public bool shouldDestroy;
        public int ID;
        private float timeAlive = 0f;
        private Vector3 currentPosition;
        private Vector3 currentDirection;
        public SimBullet SimBullet;
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
            if (isClient)
            {
                ClientVisualUpdate();
            }
        }

        void FixedUpdate()
        {
           if (isServer)
            {
                ServerPhysicsUpdate();
            }
        }

        [ServerCallback]
        void ServerPhysicsUpdate()
        {

            List<ReturnPositionalValues> points = new();
            float currTime = SimBullet.travelTime;
            float step = Time.fixedDeltaTime / SimBullet.resolution;

            for (int i = 0; i < SimBullet.resolution; i++)
            {
                float prevTime = currTime;
                currTime += step;
                ReturnPositionalValues pos;

                switch (SimBullet.type)
                {
                    case SimBulletType.linear:
                        LinearDrop.Evaluate(SimBullet, prevTime, currTime, out pos);
                        break;
                    case SimBulletType.logarithmic:
                        LogarithmicDrop.Evaluate(SimBullet, prevTime, currTime, out pos);
                        break;
                    case SimBulletType.exponential:
                        ExponentialDrop.Evaluate(SimBullet, prevTime, currTime, out pos);
                        break;
                    default:
                        continue;
                }
                points.Add(pos);
            }

            SimBullet.travelTime = currTime;

            bool expired = IsExpired(points, SimBullet, out var hits, out var endPoint);

            foreach (var hit in hits)
            {
                if (hit.collider == null)
                    continue;

                HurtBox hurtbox = hit.collider.GetComponentInParent<HurtBox>();
                if (hurtbox == null || hurtbox.healthComponent == null)
                    continue;

                var hc = hurtbox.healthComponent;

                if (SimBullet.hitHealthComponents.Contains(hc))
                    continue;

                var attackerBody = SimBullet.damageInfo.attacker?.GetComponent<CharacterBody>();
                if (attackerBody != null && hc.body == attackerBody)
                    continue;

                if (hurtbox.teamIndex == SimBullet.damageInfo.attacker?.GetComponent<TeamComponent>()?.teamIndex)
                    continue;

                var dmg = ConstructDamageInfoFromHurtbox(SimBullet.damageInfo, hurtbox, hit);
                if (dmg != null)
                {
                    dmg.inflictor = hc.gameObject;
                    hc.TakeDamage(dmg);
                }

                SimBullet.hitHealthComponents.Add(hc);

                if (SimBullet.explodeOnPassthrough)
                {
                    SimBullet.Explode(hit.point, hit.normal);
                }
                break;
            }

            if (expired)
            {
                Vector3 pos = endPoint.collider != null
                    ? endPoint.point
                    : (points.Count > 0 ? points[^1].currentPosition : SimBullet.origin);

                if (SimBullet.explodeOnExpire)
                {
                    SimBullet.Explode(pos);
                }
            }
        }

        DamageInfo ConstructDamageInfoFromHurtbox(DamageInfo sourceDamageInfo, HurtBox hurtbox, RaycastHit hit)
        {
            if (sourceDamageInfo == null || hurtbox == null || hurtbox.healthComponent == null)
                return null;

            return new DamageInfo
            {
                attacker = sourceDamageInfo.attacker,
                inflictor = sourceDamageInfo.inflictor ?? gameObject,
                damage = sourceDamageInfo.damage,
                crit = sourceDamageInfo.crit,
                damageType = sourceDamageInfo.damageType,
                procCoefficient = sourceDamageInfo.procCoefficient > 0 ? sourceDamageInfo.procCoefficient : 1f,
                procChainMask = sourceDamageInfo.procChainMask,
                position = hit.point,
                force = sourceDamageInfo.force,
                damageColorIndex = sourceDamageInfo.damageColorIndex,
                rejected = false
            };
        }

        [ClientCallback]
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

            if (SimBullet?.tracerPrefab)
            {
                var evaluation = new List<ReturnPositionalValues>();
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

        void ReturnEvaluation(out ReturnPositionalValues update, float newTime, float prevTimeOverride = -1f)
        {
            float prevTime = prevTimeOverride >= 0f ? prevTimeOverride : timeAirborne;

            switch (type)
            {
                case SimBulletType.linear:
                    LinearDrop.Evaluate(SimBullet, prevTime, newTime, out update);
                    break;
                case SimBulletType.logarithmic:
                    LogarithmicDrop.Evaluate(SimBullet, prevTime, newTime, out update);
                    break;
                case SimBulletType.exponential:
                    ExponentialDrop.Evaluate(SimBullet, prevTime, newTime, out update);
                    break;
                case SimBulletType.realisticGravity:
                    LinearDrop.Evaluate(SimBullet, prevTime, newTime, out update);
                    break;
                default:
                    LinearDrop.Evaluate(SimBullet, prevTime, newTime, out update);
                    break;
            }
        }
    }
}