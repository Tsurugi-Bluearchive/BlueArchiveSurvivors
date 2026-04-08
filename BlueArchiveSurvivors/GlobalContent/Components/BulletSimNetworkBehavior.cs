using BAMod.GlobalContent.Scripts;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static BAMod.GlobalContent.Scripts.SimulatedBulletAttack;

namespace BAMod.GlobalContent.Components
{
    internal class BulletSimNetworkBehavior : NetworkBehaviour
    {
        public static Dictionary<int, SimBullet> ServerBullets = new();
        public static Dictionary<int, ClientSimBullet> ClientBullets = new();
        public static List<(int key, RaycastHit hit)> PendingDestroy = new();

        public static BulletSimNetworkBehavior ServerInstance;

        public static float PhysicsStep;
        public static float PhysicsTime;

        private int _nextBulletId = 1;

        [ServerCallback]
        void ServerPhysicsUpdate()
        {
            if (PendingDestroy.Count > 0)
            {
                foreach (var i in PendingDestroy)
                {
                    ServerBullets.Remove(i.key);
                    RPCExpireAttack(i.key);
                }
                PendingDestroy.Clear();
            }

            foreach (var bullet in ServerBullets)
            {
                var simBullet = bullet.Value;
                List<ReturnPositionalValues> points = new();
                PhysicsStep = Time.fixedDeltaTime / simBullet.resolution;

                for (int i = 0; i < simBullet.resolution; i++)
                {
                    simBullet.attack.Evaluate(
                        out var newPosition,
                        simBullet.travelTime,
                        simBullet.travelTime + PhysicsStep * ((i + 1) / (float)simBullet.resolution)
                    );
                    points.Add(newPosition);
                }

                RaycastHit[] hits = Array.Empty<RaycastHit>();
                if (isExpired(points, simBullet, out hits, out var endPoint))
                {
                    PendingDestroy.Add((bullet.Key, endPoint));
                }

                foreach (var hit in hits)
                {
                    if (hit.collider.gameObject.TryGetComponent<HurtBox>(out var hurtbox) &&
                        hurtbox.healthComponent &&
                        hurtbox.healthComponent.body)
                    {
                        DamageInfo damageInfo = ConstructDamageInfoFromHurtbox(simBullet.damageInfo, hurtbox, hit);
                        if (damageInfo != null)
                        {
                            damageInfo.inflictor = hurtbox.healthComponent.gameObject;
                            hurtbox.healthComponent.TakeDamage(damageInfo);
                        }
                    }
                }
            }
        }

        [Command]
        void CmdRegisterBullet(AttemptDamagePacket damagePacket, Vector3 origin, Vector3 direction, float velocity, float dropSpeed, byte resolution)
        {
            int newId = _nextBulletId++;

            var bullet = new SimBullet(damagePacket.type)
            {
                origin = origin,
                direction = direction,
                velocity = velocity,
                resolution = resolution,
                dropSpeed = dropSpeed,
                damageInfo = ConstructDamageInfoFromNetworkPacket(damagePacket),
                travelTime = 0f,
            };

            ServerBullets[newId] = bullet;
            RPCSpawnBullet(newId, origin, direction);
        }

        [ClientRpc]
        void RPCSpawnBullet(int id, Vector3 origin, Vector3 direction)
        {
            if (!isClient) return;

            ClientBullets[id] = new ClientSimBullet
            {
                position = origin,
                direction = direction
            };
        }

        [ClientRpc]
        void RPCExpireAttack(int id)
        {
            if (!isClient) return;
            if (ClientBullets.TryGetValue(id, out var bullet))
            {
                if (bullet.ghostPrefab) Destroy(bullet.ghostPrefab);
                ClientBullets.Remove(id);
            }
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
            if (isServer)
            {
                if (ServerInstance != this) ServerInstance = this;
                ServerPhysicsUpdate();
            }
        }

        [ClientCallback]
        void Update()
        {
            foreach (var bullet in ClientBullets.Values)
            {
                bullet.position += bullet.direction * bullet.forwardsVelocity * Time.deltaTime;
                if (bullet.ghostPrefab)
                    bullet.ghostPrefab.transform.position = bullet.position;
            }
        }
        public void RegisterBullet(AttemptDamagePacket damagePacket, Vector3 origin, Vector3 direction, float velocity, float dropSpeed, byte resolution)
        {
            if (isClient)
            {
                CmdRegisterBullet(damagePacket, origin, direction, velocity, dropSpeed, resolution);
            }
        }
    }

    internal class ClientSimBullet
    {
        public Vector3 position;
        public Vector3 direction;
        public float forwardsVelocity;
        public GameObject ghostPrefab;
        public GameObject tracerPrefab;
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
    }

    enum SimBulletType
    {
        logarithmic,
        linear,
        exponential,
        realisticGravity
    }

}