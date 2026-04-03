using UnityEngine;
using EntityStates;
using RoR2;
using BAMod.Tsurugi.SkillStates.Special;
namespace UltrakillMod.V1.SkillStates.BaseStates
{
    public class TsurugiCharacterMain : GenericCharacterMain
    {
        /// <summary>
        /// The confirmed primary kills, resests when stock reaches zero
        /// </summary>
        public int confirmedPrimaryKills;
        /// <summary>
        /// The confirmed secondary kills, resets when stock reaches zero
        /// </summary>
        public int confirmedSecondaryKills;

        /// <summary>
        /// Dictates if primary is crit for this rack
        /// </summary>
        public bool primaryCritRack;
        /// <summary>
        /// Dictates if secondary is crit for this rack
        /// </summary>
        public bool secondaryCritRack;

        /// <summary>
        /// Tracks if primary is using mysterious rounds rather than normal ones
        /// </summary>
        public bool primaryMysterious;

        /// <summary>
        /// Tracks if secondary is using mysterious rounds rather than normal ones
        /// </summary>
        public bool secondaryMysterious;

        /// <summary>
        /// Used to reset all stocks of non-weapon skills
        /// </summary>
        public bool resetStocks;


        //TsurugiCharacterMain.cs code start
        public override void OnEnter()
        {
            base.OnEnter();

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (resetStocks)
                {
                    skillLocator.utility.stock = skillLocator.utility.maxStock;
                    skillLocator.special.stock = skillLocator.special.maxStock;
                    resetStocks = false;
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
