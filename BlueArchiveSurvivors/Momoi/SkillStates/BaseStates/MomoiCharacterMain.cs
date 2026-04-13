using UnityEngine;
using EntityStates;
using RoR2;
using BAMod.Momoi.SkillStates.Special;
using System.Collections.Generic;
namespace BAMod.Mashiro.SkillStates.BaseStates
{
    public class MomoiCharacterMain : GenericCharacterMain
    {
        public bool AutoShotty;
        //MomoiCharacterMain.cs code start
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
