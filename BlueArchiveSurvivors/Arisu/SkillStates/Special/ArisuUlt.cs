using BAMod.Arisu.SkillStates.BaseStates;
using BAMod.Mashiro.Content;
using BAMod.Mashiro.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BAMod.Arisu.Content;

namespace BAMod.Arisu.SkillStates.Special
{
    internal class ArisuUlt : BaseArisuSkillState
    {
        protected override float baseDuration => 3;
        protected override float baseFireDelay => 2;
        protected override float fireTime => 0;

        public override void OnEnter()
        {
            base.OnEnter();
            skillLocator.primary.SetSkillOverride(this.gameObject, ArisuSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.secondary.SetSkillOverride(this.gameObject, ArisuSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.utility.SetSkillOverride(this.gameObject, ArisuSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            characterBody.AddBuff(ArisuBuffs.ArisuUltShield);
            characterBody.characterMotor.enabled = false;
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
            characterBody.RemoveBuff(ArisuBuffs.ArisuUltShield);
            skillLocator.primary.SetSkillOverride(this.gameObject, ArisuSurvivor.UltBeam, GenericSkill.SkillOverridePriority.Default);
            skillLocator.secondary.UnsetSkillOverride(this.gameObject, ArisuSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.utility.UnsetSkillOverride(this.gameObject, ArisuSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            ArisuMain.ultimateGun = true;
            characterMotor.enabled = true;
            characterMotor.velocity = Vector3.zero;
            base.OnExit();
        }
    }
}
