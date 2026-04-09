using BAMod.GlobalContent.Scripts;
using BAMod.Mashiro;
using BAMod.Mashiro.Content;
using BAMod.Mashiro.SkillStates.BaseStates;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BAMod.Mashiro.SkillStates.Secondary
{
    internal class BigRound : BaseMashiroSkillState
    {
        protected override float baseDuration => 2;
        protected override float baseFireDelay => 0.5f;
        protected override float fireTime => 1;
        private bool fired = false;
        public GameObject hitEffectPrefab = FireBarrage.hitEffectPrefab;
        public GameObject tracerEffectPrefab = FireBarrage.tracerEffectPrefab;
        public DamageTypeCombo damageType = DamageType.Generic;
        private Vector3 RecoilVector;
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                var aimRay = base.GetAimRay();
                if (!fired)
                {
                    SimBulletManager.Fire(new SimBulletManager.SimBullet()
                    {
                        radius = 1f,
                        resolution = 10,
                        damageInfo = new DamageInfo()
                        {
                            damageColorIndex = DamageColorIndex.Default,
                            damage = damageStat * MashiroStaticValues.smallGunDamageCoefficient,
                            damageType = damageType,
                            attacker = this.gameObject,
                            crit = base.RollCrit()
                        },
                        direction = aimRay.direction,
                        dropSpeed = 20,
                        maximumDistance = 300,
                        origin = aimRay.origin + aimRay.direction * 2,
                        hitMask = BulletAttack.defaultHitMask,
                        stopperMask = LayerIndex.world.mask,
                        velocity = 200,
                        owner = this.gameObject,
                        prefabIndex = MashiroAssets.MashiroBigBullet,
                        type = GlobalContent.Components.SimBulletType.exponential,
                        falloffModel = BlastAttack.FalloffModel.None,
                        explodeOnExpire = true
                    });
                }
                if (fixedAge < 0.3f)
                {
                    characterBody.characterMotor.rootMotion += RecoilVector;
                }
                if (fixedAge > duration)
                {
                    outer.SetNextStateToMain();
                    return;

                }
            }

        }

        public override void OnExit()
        {
            base.OnExit();
            if (!fired)
            {
                activatorSkillSlot.AddOneStock();
            }
        }

        public static List<Vector3> ScatterVectors(
            Vector3 forward,
            int pelletCount,
            float maxAngle,
            float randomness = 0.5f
        )
        {
            List<Vector3> directions = new List<Vector3>();

            forward.Normalize();
            Quaternion baseRotation = Quaternion.LookRotation(forward);

            for (int i = 0; i < pelletCount; i++)
            {
                float t = (i + Random.value * randomness) / pelletCount;
                float spreadAngle = maxAngle * Mathf.Sqrt(t);

                float theta = (i * 137.5f) % 360f;
                theta += Random.Range(-180f, 180f) * randomness;

                Quaternion aroundForward = Quaternion.AngleAxis(theta, Vector3.forward);
                Quaternion outwardTilt = Quaternion.AngleAxis(spreadAngle, Vector3.right);

                Vector3 dir = baseRotation * (aroundForward * outwardTilt * Vector3.forward);

                dir = Vector3.Slerp(forward, dir, 0.9f);

                directions.Add(dir.normalized);
            }

            return directions;
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
