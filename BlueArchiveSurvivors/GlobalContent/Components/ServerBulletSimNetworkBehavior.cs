using BAMod.GlobalContent.Scripts;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using static BAMod.GlobalContent.Scripts.SimBulletManager;

namespace BAMod.GlobalContent.Components
{
    internal class ServerBulletSimNetworkBehavior : NetworkBehaviour
    {
        private int _nextBulletId = 1;
        private float PhysicsStep;
        public ServerSimBulletPool _bulletPool;

        [ClientRpc]
        void RpcRequestPhysicsResync(int[] keys, NetworkPhysicsResyncPacket[] packets)
        {
            if (!isClient) return;

            for (int i = 0; i < keys.Length; i++)
            {
                int key = keys[i];

                if (ClientSimBulletPool.ClientBullets.TryGetValue(key, out var pair) &&
                    pair.bulletInstance != null &&
                    pair.bulletInstance.gameObject != null)
                {
                    pair.bulletInstance.transform.position = packets[i].position;
                    pair.bulletInstance.transform.forward = packets[i].direction;
                }
                else
                {
                    ClientSimBulletPool.AddBullet(key, Instantiate(SimBulletManager.ServerInstance._bulletPool.ServerBullets[key].simBulletPrefab).GetComponent<BulletSimComponent>(), SimBulletManager.ServerInstance ._bulletPool.ServerBullets[key]);
                }
            }
        }

        [ServerCallback]
        void ServerPhysicsUpdate()
        {
            _bulletPool.ProcessPendingDestroy();

            foreach (var bulletPair in _bulletPool.ServerBullets)
            {
                var simBullet = bulletPair.Value;
                if (simBullet == null || !simBullet.active) continue;

                List<ReturnPositionalValues> points = new List<ReturnPositionalValues>();
                PhysicsStep = Time.fixedDeltaTime / simBullet.resolution;

                for (int i = 0; i < simBullet.resolution; i++)
                {
                    float prevTime = simBullet.travelTime;
                    float currTime = simBullet.travelTime + PhysicsStep;

                    ReturnPositionalValues newPosition;

                    switch (simBullet.type)
                    {
                        case SimBulletType.linear:
                            SimBulletManager.LinearDrop.Evaluate(simBullet, prevTime, currTime, out newPosition);
                            break;
                        case SimBulletType.logarithmic:
                            SimBulletManager.LogarithmicDrop.Evaluate(simBullet, prevTime, currTime, out newPosition);
                            break;
                        case SimBulletType.exponential:
                            SimBulletManager.ExponentialDrop.Evaluate(simBullet, prevTime, currTime, out newPosition);
                            break;
                        default:
                            continue;
                    }

                    points.Add(newPosition);
                }

                RaycastHit[] hits = Array.Empty<RaycastHit>();
                if (SimBulletManager.IsExpired(points, simBullet, out hits, out var endPoint))
                {
                    _bulletPool.PendingDestroy.Add((bulletPair.Key, endPoint));
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
            if (isServer)
            {
                int newId = _nextBulletId++;

                var newBullet = new SimBullet()
                {
                    origin = origin,
                    direction = direction,
                    velocity = velocity,
                    resolution = resolution,
                    dropSpeed = dropSpeed,
                    damageInfo = ConstructDamageInfoFromNetworkPacket(damagePacket),
                    travelTime = 0f,
                    type = damagePacket.type,
                    active = true
                };

                _bulletPool.ServerBullets[newId] = newBullet;

                RpcSpawnBullet(bulletIndex, newId);
            } 
        }

        [ClientRpc]
        void RpcSpawnBullet(int index, int ID)
        {
            if (!isClient) return;

            if (SimBulletManager.ReturnSimbBulletObject(out var toInstantiate, index))
            {
                var component = Instantiate(toInstantiate).GetComponent<BulletSimComponent>();
                if (component != null)
                {
                    component.ID = ID;
                }
            }
            else
            {
                Debug.LogError("Failed to instantiate SimBullet client-side!");
            }
        }

        [ClientRpc]
        void RPCExpireAttack(int id)
        {
            if (!isClient) return;
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
            var obj = Util.FindNetworkObject(ID);
            if (obj != null)
                SimBulletManager.ServerInstance = obj.GetComponent<ServerBulletSimNetworkBehavior>();
        }

        public void RegisterBullet(AttemptDamagePacket damagePacket, Vector3 origin, Vector3 direction, float velocity, float dropSpeed, byte resolution, int bulletIndex)
        {
            if (isClient)
            {
                CmdRegisterBullet(damagePacket, origin, direction, velocity, dropSpeed, resolution, bulletIndex);
            }
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (isServer)
            {
                _bulletPool.ServerBullets = new Dictionary<int, SimBullet>();
                _bulletPool.PendingDestroy = new List<(int key, RaycastHit hit)>();
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