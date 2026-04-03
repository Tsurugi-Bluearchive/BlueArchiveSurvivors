using BA.Tsurugi.SkillStates.BaseStates;
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
            if (EntityStateMachine.TryFindByCustomName(characterBody.gameObject, "ShotgunLeft", out var left))
            {
                left.SetNextState(new LockSkill());
            }
            if (EntityStateMachine.TryFindByCustomName(characterBody.gameObject, "ShotgunRight", out var right))
            {
                right.SetNextState(new LockSkill());
            }
            if (EntityStateMachine.TryFindByCustomName(characterBody.gameObject, "Movement", out var move))
            {
                move.SetNextState(new LockSkill());
            }
            characterMotor.enabled = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (FullyChanneled)
                {
                    skillLocator.primary.stock = skillLocator.primary.maxStock;
                    skillLocator.secondary.stock = skillLocator.secondary.maxStock;
                    TsurugiMain.primaryMysterious = true;
                    TsurugiMain.secondaryMysterious = true;
                    outer.SetNextStateToMain();
                    return;
                }
                else if (!Channeling)
                {
                    activatorSkillSlot.AddOneStock();
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }
        public override void OnExit()
        {
            base.OnExit();

            if (EntityStateMachine.TryFindByCustomName(characterBody.gameObject, "ShotgunLeft", out var left))
            {
                left.SetNextStateToMain();
            }
            if (EntityStateMachine.TryFindByCustomName(characterBody.gameObject, "ShotgunRight", out var right))
            {
                right.SetNextStateToMain();
            }
            if (EntityStateMachine.TryFindByCustomName(characterBody.gameObject, "Movement", out var move))
            {
                move.SetNextStateToMain();
            }
            characterMotor.enabled = true;
            characterMotor.velocity = Vector3.zero;
        }
    }
}
