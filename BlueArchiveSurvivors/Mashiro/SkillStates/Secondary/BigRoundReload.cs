using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using BAMod.Mashiro.SkillStates.BaseStates;

namespace BAMod.Mashiro.SkillStates.Secondary
{
    internal class BigRoundReload : BaseMashiroSkillState
    {
        protected override float baseDuration => 10f;
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
            skillLocator.secondary.UnsetSkillOverride(this.gameObject, MashiroSurvivor.BigRoundReload, GenericSkill.SkillOverridePriority.Default);
        }
    }
}
