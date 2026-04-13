using EntityStates.Commando.CommandoWeapon;
using R2API;
using Rewired.Demos;
using RoR2;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using EntityStates;
using BAMod.Arisu.SkillStates.BaseStates;
using BAMod.Arisu.Content;
using Rewired.ComponentControls.Data;
using UnityEngine.Android;

namespace BAMod.Arisu.SkillStates.Primary
{
    internal class BeamAttack : BaseArisuSkillState
    {
        protected override float baseDuration => 0.1f;
        protected override float baseFireDelay => 0;
        protected override float fireTime => 0;
        private bool fired = false;
        public GameObject hitEffectPrefab = FireBarrage.hitEffectPrefab;
        public GameObject tracerEffectPrefab = FireBarrage.tracerEffectPrefab;
        public DamageType damageType = DamageType.IgniteOnHit;
        private float tick;
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (ArisuMain.ultimateGun)
            {
                skillLocator.primary.AddOneStock();
                outer.SetNextStateToMain();
                return;
            }
            if (isAuthority && !ArisuMain.ultimateGun)
            {
                if (!fired)
                {
                    var aimRay = GetAimRay();
                    var beamAttack = new BulletAttack()
                    {
                        damage = (damageStat * (Mathf.Lerp(ArisuStaticValues.baseBeamDamage, ArisuStaticValues.maxBaseBeamDamage, ArisuMain.beamTime / 10f)) / 10f) / attackSpeedStat,
                        damageType = DamageType.Generic,
                        damageColorIndex = DamageColorIndex.Electrocution,
                        maxDistance = 300f,
                        _maxDistance = 300f,
                        falloffModel = BulletAttack.FalloffModel.None,
                        hitMask = BulletAttack.defaultHitMask,
                        tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("RoR2/DLC1/Railgunner/TracerRailgunCryo"),
                        radius = 2,
                        origin = aimRay.origin + aimRay.direction * 2,
                        allowTrajectoryAimAssist = true,
                        aimVector = aimRay.direction,
                        stopperMask = LayerIndex.world.mask,
                        isCrit = RollCrit(),
                        procCoefficient = 1.0f,
                        teamIndexOverride = this.teamComponent.teamIndex
                    };
                    beamAttack.Fire();
                    fired = true;
                    ArisuMain.beamTime += 0.1f;
                }

                if (fixedAge > duration)
                {
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
            if (skillLocator.primary.stock <= 0)
            {
                skillLocator.primary.SetSkillOverride(this.gameObject, ArisuSurvivor.BeamOverheat, GenericSkill.SkillOverridePriority.Default);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
