using BAMod.Saori.SkillStates.BaseStates;
using BAMod.Tsurugi.SkillStates.BaseStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAMod.Saori.SkillStates.Primary
{
    internal class BurstRifleReload : BaseSaoriSkillState
    {
        protected override float baseDuration => 2f;
        protected override float baseFireDelay => 1f;
        protected override float fireTime => 1f;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            skillLocator.secondary.stock = skillLocator.secondary.maxStock;
            skillLocator.primary.UnsetSkillOverride(this.gameObject, SaoriSurvivor.BurstRifleReload, GenericSkill.SkillOverridePriority.Default);
        }
    }
}
