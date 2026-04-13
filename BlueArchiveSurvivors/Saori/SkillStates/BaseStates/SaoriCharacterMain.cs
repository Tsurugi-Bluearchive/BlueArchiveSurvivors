using UnityEngine;
using EntityStates;
using RoR2;
using BAMod.Saori.SkillStates.Special;
using System.Collections.Generic;
namespace BAMod.Mashiro.SkillStates.BaseStates
{
    public class SaoriCharacterMain : GenericCharacterMain
    {

        public List<HealthComponent> markedHealthComponents = new();

        //SaoriCharacterMain.cs code start
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            foreach (HealthComponent healthComponent in markedHealthComponents)
            {
                if (healthComponent == null || !healthComponent.alive)
                {
                    this.healthComponent.HealFraction(0.05f, new ProcChainMask());
                }
            }
            markedHealthComponents.RemoveAll(hc => hc == null || !hc.alive);
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
