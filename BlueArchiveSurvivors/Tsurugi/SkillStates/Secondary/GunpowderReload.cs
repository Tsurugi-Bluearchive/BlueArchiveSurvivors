using BA.Tsurugi;
using RoR2;
using BA.Tsurugi.SkillStates.BaseStates;
using System;
using System.Collections.Generic;
using System.Text;

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
            activatorSkillSlot.UnsetSkillOverride(this, TsurugiSurvivor.GunpowderReload, GenericSkill.SkillOverridePriority.Default);
            activatorSkillSlot.skillDef = TsurugiSurvivor.Gunpowder;
        }
    }
}
