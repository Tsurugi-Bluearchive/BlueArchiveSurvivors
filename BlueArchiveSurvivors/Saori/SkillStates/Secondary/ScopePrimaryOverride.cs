using BAMod.Saori.Content;
using BAMod.Saori.SkillStates.BaseStates;
using BAMod.Tsurugi.SkillStates.BaseStates;
using EntityStates.Commando.CommandoWeapon;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BAMod.Saori.SkillStates.Secondary
{
    internal class ScopePrimaryOverride : BaseSaoriSkillState
    {
        protected override float baseDuration => 2f;
        protected override float baseFireDelay => 0f;
        protected override float fireTime => 0f;

        public GameObject hitEffectPrefab = FireBarrage.hitEffectPrefab;
        public GameObject tracerEffectPrefab = FireBarrage.tracerEffectPrefab;
        public DamageTypeCombo damageType = DamageType.Generic;
        private bool fired;
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            var aimRay = GetAimRay();
            if (!fired)
            {
                new BulletAttack()
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = base.characterBody.spreadBloomAngle,
                    bulletCount = 1U,
                    procCoefficient = 1f,
                    damage = base.characterBody.damage * SaoriStaticValues.scopedDamage,
                    force = 3,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    tracerEffectPrefab = this.tracerEffectPrefab,
                    hitEffectPrefab = this.hitEffectPrefab,
                    isCrit = true,
                    HitEffectNormal = false,
                    stopperMask = BulletAttack.defaultStopperMask,
                    smartCollision = true,
                    maxDistance = 300f,
                    damageType = damageType,
                    radius = 1
                }.Fire();
                fired = true;
            }
            if (fixedAge > duration && fired)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
