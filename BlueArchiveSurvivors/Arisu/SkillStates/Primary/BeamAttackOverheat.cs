using BAMod.Arisu.Content;
using BAMod.Arisu.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BAMod.Arisu.SkillStates.Primary
{
    internal class BeamAttackOverheat : BaseArisuSkillState
    {
        protected override float baseDuration => 0;
        protected override float baseFireDelay => 0;
        protected override float fireTime => 0;
        private float tick;
        private float damageTick;
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            tick += Time.fixedDeltaTime;
            damageTick += Time.fixedDeltaTime;
            if (isAuthority)
            {
                if (damageTick > 0.2f)
                {
                    characterBody.AddBuff(ArisuBuffs.ArisuOverheatStack);
                    damageTick -= 0.2f;
                }
                if (tick > 0.1f)
                {
                    var aimRay = GetAimRay();
                    var beamAttack = new BulletAttack()
                    {
                        damage = ((damageStat * ArisuStaticValues.maxBaseBeamDamage) / 10f) / attackSpeedStat,
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
                        allowTrajectoryAimAssist = true,
                        stopperMask = LayerIndex.world.mask,
                        isCrit = RollCrit(),
                        procCoefficient = 1.0f,
                        owner = this.gameObject
                    };
                    beamAttack.Fire();
                    tick -= 0.1f;
                }
                else if (!IsKeyDownAuthority())
                {
                    skillLocator.primary.UnsetSkillOverride(this.gameObject, ArisuSurvivor.BeamOverheat, GenericSkill.SkillOverridePriority.Default);
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
