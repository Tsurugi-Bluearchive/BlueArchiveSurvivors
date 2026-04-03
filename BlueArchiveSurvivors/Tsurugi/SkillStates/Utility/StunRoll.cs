using BA.Tsurugi.SkillStates.BaseStates;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using RoR2;

namespace BAMod.Tsurugi.SkillStates.Utility
{
    internal class StunRoll : BaseTsurugiSkillState
    {
        protected override float baseFireDelay => 0f;
        protected override float baseDuration => 0.5f;

        protected override float fireTime => 0f;

        private float tick;
        public override void OnEnter()
        {
            base.OnEnter();
            characterBody.fakeActorCounter += 1;
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
            base.OnExit();
            characterBody.fakeActorCounter -= 1;
        }

        private Vector3 GetMoveVector()
        {
            Vector3 moveVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            moveVector = moveVector * (10 * moveSpeedStat * GetDeltaTime());
            return moveVector;
        }
    }
}
