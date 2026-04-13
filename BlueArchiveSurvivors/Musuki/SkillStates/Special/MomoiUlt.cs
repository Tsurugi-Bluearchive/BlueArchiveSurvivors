using BAMod.Mutsuki.SkillStates.BaseStates;
using BAMod.Mashiro.Content;
using BAMod.Mashiro.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BAMod.Mutsuki.Content;

namespace BAMod.Mutsuki.SkillStates.Special
{
    internal class MutsukiUlt : BaseMutsukiSkillState
    {
        protected override float baseDuration => 3;
        protected override float baseFireDelay => 2;
        protected override float fireTime => 0;

        public override void OnEnter()
        {
            base.OnEnter();
            skillLocator.primary.SetSkillOverride(this.gameObject, MutsukiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.secondary.SetSkillOverride(this.gameObject, MutsukiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.utility.SetSkillOverride(this.gameObject, MutsukiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            characterMotor.enabled = false;
            characterBody.AddBuff(MutsukiBuffs.MutsukiUltShield);
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
            characterBody.RemoveBuff(MutsukiBuffs.MutsukiUltShield);
            skillLocator.primary.UnsetSkillOverride(this.gameObject, MutsukiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.secondary.UnsetSkillOverride(this.gameObject, MutsukiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.utility.UnsetSkillOverride(this.gameObject, MutsukiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            characterMotor.enabled = true;
            characterMotor.velocity = Vector3.zero;
            MutsukiMain.AutoShotty = true;
            base.OnExit();
        }
    }
}
