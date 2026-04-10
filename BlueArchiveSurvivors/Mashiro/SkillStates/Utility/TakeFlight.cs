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
            characterBody.AddBuff(LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility"));
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!isAuthority)
                return;

            if (fixedAge >= baseDuration)
            {
                outer.SetNextStateToMain();
                return;
            }

            LayerMask ceilingMask = LayerIndex.world.mask | LayerIndex.triggerZone.mask;

            Ray upwardRay = new Ray(characterBody.corePosition, Vector3.up);

            if (Physics.Raycast(upwardRay, out RaycastHit hit, 20f, ceilingMask))
            {
                outer.SetNextStateToMain();
                return;
            }

            characterMotor.velocity = Vector3.zero;
            characterMotor.rootMotion += new Vector3(0f, 15f, 0f);
        }

        public override void OnExit()
        {
            characterBody.fakeActorCounter -= 1;
            characterBody.RemoveBuff(LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility"));
            base.OnExit();
        }
    }
}
