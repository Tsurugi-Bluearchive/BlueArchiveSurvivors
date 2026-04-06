using BAMod.Mashiro.SkillStates.BaseStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAMod.Mashiro.SkillStates.Secondary
{
    internal class Scope : BaseMashiroSkillState
    {
        protected override float baseDuration => 0f;
        protected override float baseFireDelay => 0f;
        protected override float fireTime => 0f;

        public override void OnEnter()
        {
            base.OnEnter();
            skillLocator.primary.SetSkillOverride(this.gameObject, MashiroSurvivor.BigRound, GenericSkill.SkillOverridePriority.Default);
        }
        public override void FixedUpdate()
        {
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
            skillLocator.primary.UnsetSkillOverride(this.gameObject, MashiroSurvivor.BigRound, GenericSkill.SkillOverridePriority.Default);
            if (skillLocator.primary.stock <= 0)
            {
                skillLocator.secondary.SetSkillOverride(this.gameObject, MashiroSurvivor.BigRoundReload, GenericSkill.SkillOverridePriority.Default);
            }
        }
    }
}
