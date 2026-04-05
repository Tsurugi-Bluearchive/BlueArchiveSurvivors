using BAMod.Saori.Content;
using BAMod.Saori.SkillStates.BaseStates;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using R2API;
using RoR2;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BAMod.Saori.SkillStates.Secondary
{
    internal class Scope : BaseSaoriSkillState
    {
        protected override float baseDuration => 4;
        protected override float baseFireDelay => 0.5f;
        protected override float fireTime => 1;
        private bool fired = false;
        public GameObject hitEffectPrefab = FireBarrage.hitEffectPrefab;
        public GameObject tracerEffectPrefab = FireBarrage.tracerEffectPrefab;
        public DamageTypeCombo damageType = DamageType.Generic;
        public override void OnEnter()
        {
            base.OnEnter();
            skillLocator.primary.SetSkillOverride(this.gameObject, SaoriSurvivor.ScopePrimaryOverride, GenericSkill.SkillOverridePriority.Default);
            skillLocator.primary.OverrideMaxStock(skillLocator.secondary.stock * 3);
            skillLocator.primary.stock = skillLocator.primary.maxStock;
            skillLocator.secondary.stock = 0;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (!IsKeyDownAuthority())
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            skillLocator.secondary.stock = skillLocator.primary.stock > 0 ? Mathf.RoundToInt(skillLocator.primary.stock / 3) : 0;
            skillLocator.primary.UnsetSkillOverride(this.gameObject, SaoriSurvivor.ScopePrimaryOverride, GenericSkill.SkillOverridePriority.Default);
            if (skillLocator.secondary.stock <= 0)
            {
                skillLocator.primary.SetSkillOverride(this.gameObject, SaoriSurvivor.BurstRifleReload, GenericSkill.SkillOverridePriority.Default);
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
