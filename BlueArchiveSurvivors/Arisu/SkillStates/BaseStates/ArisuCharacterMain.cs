using BAMod.Arisu.Content;
using BAMod.Arisu.SkillStates.Primary;
using BAMod.Arisu.SkillStates.Special;
using BAMod.Arisu.SkillStates.Utility;
using EntityStates;
using IL.RoR2.Achievements.FalseSon;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
namespace BAMod.Arisu.SkillStates.BaseStates
{
    public class ArisuCharacterMain : GenericCharacterMain
    {
        public float beamTime;
        public bool rooted;
        public bool ultimateGun;
        private bool ultimateGunOverridden;
        //ArisuCharacterMain.cs code start
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            if (!isAuthority) return;

            base.FixedUpdate();


            if (EntityStateMachine.TryFindByCustomName(this.gameObject, "Gun", out var gun) &&
                EntityStateMachine.TryFindByCustomName(this.gameObject, "Movement", out var utility) &&
                EntityStateMachine.TryFindByCustomName(this.gameObject, "Ult", out var ult))
            {
                if ((gun.state.GetType() == typeof(BeamAttack) || gun.state.GetType() == typeof(ArisuUltBeamAttack) ||
                    gun.state.GetType() == typeof(ArisuUltBeamAttack) || gun.state.GetType() == typeof(ArisuUltBeamAttackOverheat))
                    && utility.state.GetType() != typeof(EmergencyCooling))
                {
                    if(!rooted) characterBody.AddBuff(ArisuBuffs.Withstand);
                    characterBody.characterMotor.enabled = false;
                    rooted = true;
                }
                else if (ult.GetType() == typeof(ArisuUlt))
                {
                    characterBody.characterMotor.enabled = false;
                    rooted = true;
                }
                else if ((rooted && !inputBank.skill1.down) || utility.state.GetType() == typeof(EmergencyCooling))
                {
                    if (characterBody.HasBuff(ArisuBuffs.Withstand))
                    {
                        characterBody.RemoveBuff(ArisuBuffs.Withstand);
                    }
                    beamTime = 0;
                    rooted = false;
                    characterBody.characterMotor.enabled = true;
                    characterBody.characterMotor.velocity = Vector3.zero;
                }
            }

        }
        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Vehicle;
        }

    }
}
