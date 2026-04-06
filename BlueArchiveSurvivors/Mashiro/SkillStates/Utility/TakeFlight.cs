using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using RoR2;
using BAMod.Mashiro.SkillStates.BaseStates;

namespace BAMod.Mashiro.SkillStates.Utility
{
    internal class TakeFlight : BaseMashiroSkillState
    {
        protected override float baseFireDelay => 0f;
        protected override float baseDuration => 0.2f;

        protected override float fireTime => 0f;

        private float tick;
        public override void OnEnter()
        {
            base.OnEnter();
            characterBody.fakeActorCounter += 1;
            characterBody.AddBuff(LegacyResourcesAPI.Load<BuffDef>("RoR2/Base/Common/bdHiddenInvincibility"));
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (fixedAge < baseDuration)
                {
                    characterMotor.velocity = Vector3.zero;
                    characterMotor.rootMotion += new Vector3(0, 20, 0);
                }
                else
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            characterBody.fakeActorCounter -= 1;
            characterBody.RemoveBuff(LegacyResourcesAPI.Load<BuffDef>("RoR2/Base/Common/bdHiddenInvincibility"));
            base.OnExit();
        }
    }
}
