using BAMod.Arisu.Content;
using BAMod.Arisu.SkillStates.BaseStates;
using BAMod.Mashiro.Content;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BAMod.Arisu.SkillStates.Special
{
    internal class ArisuUltBeamAttack : BaseArisuSkillState
    {
        protected override float baseDuration => 0.1f;
        protected override float baseFireDelay => 0;
        protected override float fireTime => 0;

        private bool fired = false;
        private float tick;
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (!fired)
                {
                    var aimRay = GetAimRay();
                    var beamAttack = new BulletAttack()
                    {
                        damage = (damageStat * (Mathf.Lerp(ArisuStaticValues.ultBeamDamage, ArisuStaticValues.maxUltBeamDamage, ArisuMain.beamTime / 10f)) / 10f) / attackSpeedStat,
                        damageType = DamageType.Generic,
                        damageColorIndex = DamageColorIndex.Electrocution,
                        maxDistance = 300f,
                        _maxDistance = 300f,
                        falloffModel = BulletAttack.FalloffModel.None,
                        hitMask = BulletAttack.defaultHitMask,
                        tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("RoR2/DLC1/Railgunner/TracerRailgunCryo"),
                        radius = 2,
                        origin = aimRay.origin + aimRay.direction * 2,
                        aimVector = aimRay.direction,
                        stopperMask = LayerIndex.world.mask,
                        isCrit = RollCrit(),
                        procCoefficient = 1.0f,
                        owner = this.gameObject
                    };
                    beamAttack.Fire();
                    fired = true;
                    ArisuMain.beamTime += 0.1f;
                }

                if (fixedAge > duration)
                {
                    if (skillLocator.primary.stock <= 0)
                    {
                        skillLocator.primary.SetSkillOverride(this.gameObject, ArisuSurvivor.UltBeamOverheat, GenericSkill.SkillOverridePriority.Default);
                    }
                    outer.SetNextStateToMain();
                    return;
                }
                else if (!IsKeyDownAuthority())
                {
                    activatorSkillSlot.AddOneStock();
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
