using BAMod.GlobalContent.Scripts;
using HarmonyLib;
using Newtonsoft.Json.Utilities;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static BAMod.GlobalContent.Scripts.SimBulletManager;

namespace BAMod.GlobalContent.Components
{
    internal class ServerBulletSimNetworkBehavior : NetworkBehaviour
    {
        private int _nextBulletId = 1;
        public ServerSimBulletPool _bulletPool = new();
        private Queue<(int id, SimBullet bullet, int prefabIndex)> _pendingBullets = new();

        [ServerCallback]
        void ServerPhysicsUpdate()
        {
            // Spawn pending bullets
            while (_pendingBullets.Count > 0)
            {
                var (id, bullet, prefabIndex) = _pendingBullets.Dequeue();
                _bulletPool.ServerBullets[id] = bullet;

                RpcSpawnBullet(id, bullet.origin, bullet.direction, bullet.velocity, bullet.dropSpeed, bullet.resolution, prefabIndex, bullet.type);
            }

            var bulletSnapshot = new List<KeyValuePair<int, SimBullet>>(_bulletPool.ServerBullets);

            foreach (var bulletPair in bulletSnapshot)
            {
                var simBullet = bulletPair.Value;
                if (simBullet == null || !simBullet.active)
                    continue;

                List<ReturnPositionalValues> points = new();
                float currTime = simBullet.travelTime;
                float step = Time.fixedDeltaTime / simBullet.resolution;

                for (int i = 0; i < simBullet.resolution; i++)
                {
                    float prevTime = currTime;
                    currTime += step;

                    ReturnPositionalValues pos;

                    switch (simBullet.type)
                    {
                        case SimBulletType.linear:
                            LinearDrop.Evaluate(simBullet, prevTime, currTime, out pos);
                            break;
                        case SimBulletType.logarithmic:
                            LogarithmicDrop.Evaluate(simBullet, prevTime, currTime, out pos);
                            break;
                        case SimBulletType.exponential:
                            ExponentialDrop.Evaluate(simBullet, prevTime, currTime, out pos);
                            break;
                        default:
                            continue;
                    }

                    points.Add(pos);
                }

                simBullet.travelTime = currTime;

                bool expired = IsExpired(points, simBullet, out var hits, out var endPoint);

                foreach (var hit in hits)
                {
                    if (hit.collider == null)
                        continue;

                    HurtBox hurtbox = hit.collider.GetComponentInParent<HurtBox>();
                    if (hurtbox == null || hurtbox.healthComponent == null)
                        continue;

                    var hc = hurtbox.healthComponent;

                    if (simBullet.hitHealthComponents.Contains(hc))
                        continue;

                    var attackerBody = simBullet.damageInfo.attacker?.GetComponent<CharacterBody>();

                    if (attackerBody != null && hc.body == attackerBody)
                        continue;

                    if (hurtbox.teamIndex == simBullet.damageInfo.attacker?.GetComponent<TeamComponent>()?.teamIndex)
                        continue;

                    var dmg = ConstructDamageInfoFromHurtbox(simBullet.damageInfo, hurtbox, hit);
                    if (dmg != null)
                    {
                        dmg.inflictor = hc.gameObject;
                        hc.TakeDamage(dmg);
                    }

                    simBullet.hitHealthComponents.Add(hc);

                    if (simBullet.explodeOnPassthrough)
                    {
                        simBullet.Explode(hit.point, hit.normal);
                    }

                    break;
                }

                if (expired)
                {
                    Vector3 pos = endPoint.collider != null
                        ? endPoint.point
                        : (points.Count > 0 ? points[^1].currentPosition : simBullet.origin);

                    if (simBullet.explodeOnExpire)
                    {
                        simBullet.Explode(pos);
                    }

                    _bulletPool.PendingDestroy.Enqueue((bulletPair.Key, endPoint));
                }
            }

            _bulletPool.ProcessPendingDestroy();
        }

        [ClientRpc]
        void RpcDestroyClientInstances(int index)
        {
            ClientSimBulletPool.DestroyClientBullets.Enqueue(index);
        }

        [Command]
        void CmdRegisterBullet(
            AttemptDamagePacket packet,
            Vector3 origin,
            Vector3 direction,
            float velocity,
            float dropSpeed,
            byte resolution,
            int bulletIndex,
            LayerMask hitMask,
            LayerMask stopperMask,
            float radius,
            int prefabIndex)
        {
            int id = _nextBulletId++;

            var bullet = new SimBullet
            {
                origin = origin,
                direction = direction,
                velocity = velocity,
                resolution = resolution,
                dropSpeed = dropSpeed,
                damageInfo = ConstructDamageInfoFromNetworkPacket(packet),
                travelTime = 0f,
                type = packet.type,
                active = true,
                hitMask = hitMask,
                stopperMask = stopperMask,
                radius = radius,

                explodeOnPassthrough = packet.explodeOnPassthrough,
                explodeOnExpire = packet.explodeOnExpire,
                explosionRadius = packet.explosionRadius,
                explosionDamage = packet.explosionDamage,
                explosionDamageType = packet.explosionDamageType,
                explosionProcCoefficient = packet.explosionProcCoefficient,
                explosionForce = packet.explosionForce,
                falloffModel = packet.falloff
            };

            _pendingBullets.Enqueue((id, bullet, prefabIndex));
        }

        [ClientRpc]
        void RpcSpawnBullet(int id, Vector3 origin, Vector3 direction, float velocity, float dropSpeed, byte resolution, int prefabIndex, SimBulletType type)
        {
            if (!isClient) return;

            if (!SimBulletManager.ReturnSimbBulletObject(out var prefab, prefabIndex))
                return;

            var obj = Instantiate(prefab);
            var comp = obj.GetComponent<BulletSimComponent>();

            if (comp != null)
            {
                comp.ID = id;
                comp.type = type;

                comp.SimBullet = new SimBullet
                {
                    origin = origin,
                    direction = direction,
                    velocity = velocity,
                    resolution = resolution,
                    dropSpeed = dropSpeed,
                    travelTime = 0f,
                    type = type,
                    active = true
                };
            }

            ClientSimBulletPool.AddBullet(id, comp, comp.SimBullet);
        }

        DamageInfo ConstructDamageInfoFromNetworkPacket(AttemptDamagePacket packet)
        {
            return new DamageInfo
            {
                attacker = RoR2.Util.FindNetworkObject(packet.AttackerNetworkID)?.gameObject,
                inflictor = RoR2.Util.FindNetworkObject(packet.InflictorNetworkID)?.gameObject,
                damage = packet.Damage,
                crit = packet.crit,
                procCoefficient = packet.procCoefficient,
                procChainMask = packet.procChainMask,
                force = packet.force,
                damageColorIndex = packet.colorIndex,
                rejected = false
            };
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

        void FixedUpdate()
        {
            if (isServer && NetworkServer.active)
            {
                ServerPhysicsUpdate();
            }
        }

        public void RegisterBullet(
            AttemptDamagePacket packet,
            Vector3 origin,
            Vector3 direction,
            float velocity,
            float dropSpeed,
            byte resolution,
            int bulletIndex,
            LayerMask hitMask,
            LayerMask stopperMask,
            float radius,
            int prefabIndex)
        {
            if (isClient && hasAuthority)
                CmdRegisterBullet(packet, origin, direction, velocity, dropSpeed, resolution, bulletIndex, hitMask, stopperMask, radius, prefabIndex);
        }

        void Awake()
        {
            if (SimBulletManager.ServerInstance != null)
            {
                Destroy(SimBulletManager.ServerInstance.gameObject);
            }
            SimBulletManager.ServerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    internal struct AttemptDamagePacket
    {
        public NetworkInstanceId AttackerNetworkID;
        public NetworkInstanceId InflictorNetworkID;
        public float Damage;
        public bool crit;
        public float procCoefficient;
        public ProcChainMask procChainMask;
        public Vector3 force;
        public DamageColorIndex colorIndex;
        public SimBulletType type;

        public bool explodeOnPassthrough;
        public bool explodeOnExpire;
        public float explosionRadius;
        public float explosionDamage;
        public DamageTypeCombo explosionDamageType;
        public float explosionProcCoefficient;
        public float explosionForce;
        public BlastAttack.FalloffModel falloff;
    }

    internal struct NetworkPhysicsResyncPacket
    {
        public Vector3 position;
        public Vector3 direction;
    }

    enum SimBulletType
    {
        logarithmic,
        linear,
        exponential,
        realisticGravity
    }
}