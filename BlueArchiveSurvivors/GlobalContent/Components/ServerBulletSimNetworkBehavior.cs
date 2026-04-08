using BAMod.GlobalContent.Scripts;
using RoR2;
using RoR2BepInExPack.GameAssetPaths;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static BAMod.GlobalContent.Scripts.SimBulletManager;

namespace BAMod.GlobalContent.Components
{
    internal class ServerBulletSimNetworkBehavior : NetworkBehaviour
    {
        public static Dictionary<int, SimBullet> ServerBullets = new();
        public static Dictionary<int, BulletSimComponent> ClientBullets = new();
        public static List<(int key, RaycastHit hit)> PendingDestroy = new();

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
                    float prevTime = simBullet.travelTime;
                    float currTime = simBullet.travelTime + PhysicsStep * ((i + 1) / (float)simBullet.resolution);

                    ReturnPositionalValues newPosition;

                    switch (simBullet.type)
                    {
                        case SimBulletType.linear:
                            SimBulletManager.LinearDrop.Evaluate(
                                simBullet,
                                prevTime,
                                currTime,
                                out newPosition
                            );
                            break;

                        case SimBulletType.logarithmic:
                            SimBulletManager.LogarithmicDrop.Evaluate(
                                simBullet,
                                prevTime,
                                currTime,
                                out newPosition
                            );
                            break;

                        case SimBulletType.exponential:
                            SimBulletManager.ExponentialDrop.Evaluate(
                                simBullet,
                                prevTime,
                                currTime,
                                out newPosition
                            );
                            break;
                        default:
                            continue;
                    }

                    points.Add(newPosition);
                }

                RaycastHit[] hits = Array.Empty<RaycastHit>();
                if (SimBulletManager.IsExpired(points, simBullet, out hits, out var endPoint))
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
        void CmdRegisterBullet(AttemptDamagePacket damagePacket, Vector3 origin, Vector3 direction, float velocity, float dropSpeed, byte resolution, int bulletIndex)
        {
            int newId = _nextBulletId++;

            var bullet = new SimBullet()
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
            RpcSpawnBullet(bulletIndex, newId);
        }

        [ClientRpc]
        void RpcSpawnBullet(int index, int ID)
        {
            if (!isClient) return;
            if (SimBulletManager.ReturnSimbBulletObject(out var toInstantiate, index))
            {
                ClientBullets.Add(ID, Instantiate(toInstantiate, this.gameObject.transform).GetComponent<BulletSimComponent>());
                ClientBullets[ID].ID = ID;
            }
            else
            {
                Log.Error("Attempted to instantiate a SimBullet clientSide but couldn't!");
            }
        }

        [ClientRpc]
        void RPCExpireAttack(int id)
        {
            if (!isClient) return;
            if (ClientBullets.TryGetValue(id, out var component))
            {
                component.Destroy = true;
            }
            else
            {
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
                if (SimBulletManager.ServerInstance != this)
                {
                    SimBulletManager.ServerInstance = this;
                    RpcSetClientInstances(this.netId);
                }
                ServerPhysicsUpdate();
            }
        }

        [ClientRpc]
        void RpcSetClientInstances(NetworkInstanceId ID)
        {
            SimBulletManager.ServerInstance = Util.FindNetworkObject(ID).GetComponent<ServerBulletSimNetworkBehavior>();
        }
        public void RegisterBullet(AttemptDamagePacket damagePacket, Vector3 origin, Vector3 direction, float velocity, float dropSpeed, byte resolution, int bulletIndex)
        {
            if (isClient)
            {
                CmdRegisterBullet(damagePacket, origin, direction, velocity, dropSpeed, resolution, bulletIndex);
            }
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
    }

    enum SimBulletType
    {
        logarithmic,
        linear,
        exponential,
        realisticGravity
    }

}