using BAMod.Tsurugi.Content;
using BAMod.Tsurugi.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BAMod.Tsurugi.SkillStates.Special
{
    internal class TsurugiUlt : BaseTsurugiSkillState
    {
        protected override float baseDuration => 3;
        protected override float baseFireDelay => 2;
        protected override float fireTime => 0;

        public override void OnEnter()
        {
            base.OnEnter();
            TsurugiMain.primaryStock = skillLocator.primary.stock;
            TsurugiMain.secondaryStock = skillLocator.secondary.stock;
            skillLocator.primary.SetSkillOverride(this.gameObject, TsurugiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.secondary.SetSkillOverride(this.gameObject, TsurugiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.utility.SetSkillOverride(this.gameObject, TsurugiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            characterMotor.enabled = false;
            characterBody.AddBuff(TsurugiBuffs.TsurugiUltShield);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                characterBody.fakeActorCounter += 1;
                if (fixedAge > duration)
                {
                    TsurugiMain.primaryMysterious = true;
                    TsurugiMain.secondaryMysterious = true;
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }
        public override void OnExit()
        {
            base.OnExit();

            skillLocator.primary.UnsetSkillOverride(this.gameObject, TsurugiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.secondary.UnsetSkillOverride(this.gameObject, TsurugiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            skillLocator.utility.UnsetSkillOverride(this.gameObject, TsurugiSurvivor.Lock, GenericSkill.SkillOverridePriority.Default);
            characterMotor.enabled = true;
            characterMotor.velocity = Vector3.zero;
            characterBody.RemoveBuff(TsurugiBuffs.TsurugiUltShield);
        }
    }
}
