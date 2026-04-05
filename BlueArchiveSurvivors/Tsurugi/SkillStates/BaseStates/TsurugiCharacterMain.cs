using UnityEngine;
using EntityStates;
using RoR2;
using BAMod.Tsurugi.SkillStates.Special;
namespace BAMod.Tsurugi.SkillStates.BaseStates
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
        
        public int primaryStock;

        public int secondaryStock;

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

        /// <summary>
        /// Checked every frame, add to this to heal her accurately.
        /// </summary>
        public float HealBy;

        private ItemDef Magazine;

        //TsurugiCharacterMain.cs code start
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
                if (resetStocks)
                {
                    skillLocator.utility.stock = skillLocator.utility.maxStock;
                    skillLocator.special.stock = skillLocator.special.maxStock;
                    resetStocks = false;
                }
                var primaryAmmo = characterBody.inventory.GetItemCountEffective(Magazine) + 5;
                if (skillLocator.primary.maxStock != primaryAmmo)
                {
                    skillLocator.primary.OverrideMaxStock(primaryAmmo);
                }
                if (HealBy > 0)
                {
                    healthComponent.Heal(HealBy, new ProcChainMask());
                    HealBy = 0;
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
