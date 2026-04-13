using BAMod.Momoi.SkillStates.BaseStates;
using BAMod.Mashiro.Content;
using BAMod.Mashiro.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BAMod.Momoi.Content;

namespace BAMod.Momoi.SkillStates.Special
{
    internal class MomoiUlt : BaseMomoiSkillState
    {
        protected override float baseDuration => 3;
        protected override float baseFireDelay => 2;
        protected override float fireTime => 0;

        public override void OnEnter()
        {
            base.OnEnter();
            skillLocator.primary.SetSkillOverride(this.gameObject, MomoiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.secondary.SetSkillOverride(this.gameObject, MomoiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.utility.SetSkillOverride(this.gameObject, MomoiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            characterMotor.enabled = false;
            characterBody.AddBuff(MomoiBuffs.MomoiUltShield);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (fixedAge > duration)
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }
        public override void OnExit()
        {
            characterBody.RemoveBuff(MomoiBuffs.MomoiUltShield);
            skillLocator.primary.UnsetSkillOverride(this.gameObject, MomoiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.secondary.UnsetSkillOverride(this.gameObject, MomoiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.utility.UnsetSkillOverride(this.gameObject, MomoiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            characterMotor.enabled = true;
            characterMotor.velocity = Vector3.zero;
            MomoiMain.AutoShotty = true;
            base.OnExit();
        }
    }
}
