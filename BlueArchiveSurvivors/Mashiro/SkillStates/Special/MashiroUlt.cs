using BAMod.Mashiro.Content;
using BAMod.Mashiro.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BAMod.Mashiro.SkillStates.Special
{
    internal class MashiroUlt : BaseMashiroSkillState
    {
        protected override float baseDuration => 3;
        protected override float baseFireDelay => 2;
        protected override float fireTime => 0;

        public override void OnEnter()
        {
            base.OnEnter();
            skillLocator.primary.SetSkillOverride(this.gameObject, MashiroSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.secondary.SetSkillOverride(this.gameObject, MashiroSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.utility.SetSkillOverride(this.gameObject, MashiroSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            characterMotor.enabled = false;
            characterBody.AddBuff(MashiroBuffs.MashiroUltShield);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                characterBody.fakeActorCounter += 1;
                if (fixedAge > duration)
                {
                    characterBody.AddTimedBuff(MashiroBuffs.AttackEcho, 10f);
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }
        public override void OnExit()
        {
            base.OnExit();

            skillLocator.primary.UnsetSkillOverride(this.gameObject, MashiroSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.secondary.UnsetSkillOverride(this.gameObject, MashiroSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.utility.UnsetSkillOverride(this.gameObject, MashiroSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            characterMotor.enabled = true;
            characterMotor.velocity = Vector3.zero;
            characterBody.RemoveBuff(MashiroBuffs.MashiroUltShield);
        }
    }
}
