using UnityEngine;
using EntityStates;
using RoR2;
using BAMod.Saori.SkillStates.Special;
namespace BAMod.Tsurugi.SkillStates.BaseStates
{
    public class SaoriCharacterMain : GenericCharacterMain
    {

        //SaoriCharacterMain.cs code start
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
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
