using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using BAMod.Tsurugi.SkillStates.BaseStates;

namespace BAMod.Tsurugi.SkillStates.Secondary
{
    internal class GunpowderReload : BaseTsurugiSkillState
    {
        protected override float baseDuration => 1f;
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
            skillLocator.secondary.UnsetSkillOverride(this.gameObject, TsurugiSurvivor.GunpowderReload, GenericSkill.SkillOverridePriority.Default);
        }
    }
}
