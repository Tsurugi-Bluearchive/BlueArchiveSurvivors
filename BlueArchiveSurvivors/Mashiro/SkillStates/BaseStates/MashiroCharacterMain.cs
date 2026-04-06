using UnityEngine;
using EntityStates;
using RoR2;
using BAMod.Mashiro.SkillStates.Special;
namespace BAMod.Mashiro.SkillStates.BaseStates
{
    public class MashiroCharacterMain : GenericCharacterMain
    {
        /// <summary>
        /// Checked every frame, add to this to heal her accurately.
        /// </summary>
        public float HealBy;

        /// <summary>
        /// Is she currently flying?
        /// </summary>
        public bool Flying;

        private ItemDef Magazine;

        //MashiroCharacterMain.cs code start
        public override void OnEnter()
        {
            base.OnEnter();
            Magazine = LegacyResourcesAPI.Load<ItemDef>("RoR2/Base/SecondarySkillMagazine/SecondarySkillMagazine");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (HealBy > 0)
                {
                    healthComponent.Heal(HealBy, new ProcChainMask());
                    HealBy = 0;
                }
                if (Flying)
                {
                    if (inputBank.jump.justPressed && characterBody.characterMotor.jumpCount <= 0)
                    {
                        Flying = false;
                    }
                    else
                    {
                        characterBody.characterMotor.isFlying = true;
                    }
                }
                else
                {
                    if (inputBank.jump.justPressed)
                    {
                        Flying = true;
                    }
                    else
                    {
                        characterBody.characterMotor.isFlying = true;
                    }
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
