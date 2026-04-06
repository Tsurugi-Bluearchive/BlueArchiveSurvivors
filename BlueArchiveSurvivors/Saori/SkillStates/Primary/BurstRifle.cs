using EntityStates.Commando.CommandoWeapon;
using R2API;
using Rewired.Demos;
using RoR2;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using EntityStates;
using BAMod.Saori.SkillStates.BaseStates;
using BAMod.Saori.Content;

namespace BAMod.Saori.SkillStates.Primary
{
    internal class BurstRifle : BaseSaoriSkillState
    {
        protected override float baseDuration => 1;
        protected override float baseFireDelay => 0.2f;
        protected override float fireTime => 1;
        private bool fired = false;
        public GameObject hitEffectPrefab = FireBarrage.hitEffectPrefab;
        public GameObject tracerEffectPrefab = FireBarrage.tracerEffectPrefab;
        public DamageTypeCombo damageType = DamageType.Generic;
        private float tick;
        private int FiredAmount;
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                tick += Time.deltaTime;
                if (tick > 0.11f && IsKeyDownAuthority() && FiredAmount <= 6)
                {
                    var aimRay = GetAimRay();
                    var pelletVectors = ScatterVectors(aimRay.direction, 1, 3f, 1f);
                    foreach (var p in pelletVectors)
                    {
                        BulletAttack bullet = new BulletAttack
                        {
                            owner = base.gameObject,
                            weapon = base.gameObject,
                            origin = aimRay.origin,
                            aimVector = p + aimRay.direction * 2,
                            minSpread = 0f,
                            maxSpread = base.characterBody.spreadBloomAngle,
                            bulletCount = 1U,
                            procCoefficient = 1f,
                            damage = base.characterBody.damage * SaoriStaticValues.burstDamage,
                            force = 3,
                            falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                            tracerEffectPrefab = this.tracerEffectPrefab,
                            hitEffectPrefab = this.hitEffectPrefab,
                            isCrit = base.RollCrit(),
                            HitEffectNormal = false,
                            stopperMask = BulletAttack.defaultStopperMask,
                            smartCollision = true,
                            maxDistance = 300f,
                            damageType = damageType,
                            radius = 1
                        };
                        bullet.Fire();
                        tick -= 0.11f;
                        FiredAmount += 1;
                    }
                }
                if (FiredAmount >= 3 && fixedAge > duration)
                {
                    outer.SetNextStateToMain();
                    return;
                }
                else if (FiredAmount <= 3 && !IsKeyDownAuthority())
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (stock <= 0)
            {
                skillLocator.primary.SetSkillOverride(this.gameObject, SaoriSurvivor.BurstRifleReload, GenericSkill.SkillOverridePriority.Default);
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
