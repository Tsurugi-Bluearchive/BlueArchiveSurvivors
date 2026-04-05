using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using RoR2;
using BAMod.Tsurugi.SkillStates.BaseStates;
using BAMod.Saori.SkillStates.BaseStates;

namespace BAMod.Saori.SkillStates.Utility
{
    internal class StunRoll : BaseSaoriSkillState
    {
        protected override float baseFireDelay => 0f;
        protected override float baseDuration => 0.3f;
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
                    characterMotor.rootMotion += GetMoveVector();
  
                    tick++;
                    if (tick >= 10)
                    {
                        var stunSearch = new BullseyeSearch();
                        stunSearch.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
                        stunSearch.searchDirection = GetAimRay().direction;
                        stunSearch.searchOrigin = this.gameObject.transform.position;
                        stunSearch.maxAngleFilter = 360;
                        stunSearch.maxDistanceFilter = 5;
                        stunSearch.RefreshCandidates();
                        foreach (var stun in stunSearch.candidatesEnumerable)
                        {
                            if (stun.hurtBox.teamIndex != this.teamComponent.teamIndex && stun.hurtBox.healthComponent.TryGetComponent<SetStateOnHurt>(out var state))
                            {
                                state.CallRpcSetStun(5f);
                            }
                        }
                    }
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

        private Vector3 GetMoveVector()
        {
            Vector3 moveVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            moveVector = moveVector * (5 * moveSpeedStat * GetDeltaTime());
            return moveVector;
        }
    }
}
