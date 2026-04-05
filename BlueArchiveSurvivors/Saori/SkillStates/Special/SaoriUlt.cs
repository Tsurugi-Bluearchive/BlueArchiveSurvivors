using BAMod.Saori.SkillStates.BaseStates;
using BAMod.Tsurugi.Content;
using BAMod.Tsurugi.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BAMod.Saori.SkillStates.Special
{
    internal class SaoriUlt : BaseSaoriSkillState
    {
        protected override float baseDuration => 3;
        protected override float baseFireDelay => 2;
        protected override float fireTime => 0;

        public override void OnEnter()
        {
            base.OnEnter();
            skillLocator.primary.SetSkillOverride(this.gameObject, SaoriSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.secondary.SetSkillOverride(this.gameObject, SaoriSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.utility.SetSkillOverride(this.gameObject, SaoriSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            characterMotor.enabled = false;
            characterBody.AddBuff(SaoriBuffs.SaoriUltShield);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (fixedAge > duration)
                {
                    var teammates = TeamComponent.GetTeamMembers(teamComponent.teamIndex);
                    foreach (var member in teammates)
                    {
                        member.body.AddTimedBuff(SaoriBuffs.HyperCritBuff, 10f);
                    }
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            characterBody.RemoveBuff(SaoriBuffs.SaoriUltShield);
            skillLocator.primary.UnsetSkillOverride(this.gameObject, SaoriSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.secondary.UnsetSkillOverride(this.gameObject, SaoriSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.utility.UnsetSkillOverride(this.gameObject, SaoriSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            characterMotor.enabled = true;
            characterMotor.velocity = Vector3.zero;
        }
    }
}
