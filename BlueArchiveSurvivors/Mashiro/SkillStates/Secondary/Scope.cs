using BAMod.Mashiro.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace BAMod.Mashiro.SkillStates.Secondary
{
    internal class Scope : BaseMashiroSkillState
    {
        protected override float baseDuration => 0f;
        protected override float baseFireDelay => 0f;
        protected override float fireTime => 0f;

        private int cachedSecondaryStock;
        private int cachedSecondaryMaxStock;

        public override void OnEnter()
        {
            base.OnEnter();

            cachedSecondaryStock = skillLocator.secondary.stock;
            cachedSecondaryMaxStock = skillLocator.secondary.maxStock;

            skillLocator.primary.SetSkillOverride(gameObject, MashiroSurvivor.BigRound, GenericSkill.SkillOverridePriority.Default);

            skillLocator.primary.OverrideMaxStock(cachedSecondaryMaxStock);
            skillLocator.primary.stock = cachedSecondaryStock;

            skillLocator.secondary.stock = 0;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isAuthority && !IsKeyDownAuthority())
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            int remaining = skillLocator.primary.stock;

            skillLocator.primary.UnsetSkillOverride(gameObject, MashiroSurvivor.BigRound, GenericSkill.SkillOverridePriority.Default);

            skillLocator.primary.OverrideMaxStock(0);

            if (remaining <= 0)
            {
                skillLocator.secondary.SetSkillOverride(gameObject, MashiroSurvivor.BigRoundReload, GenericSkill.SkillOverridePriority.Default);
            }
            else
            {
                skillLocator.secondary.stock = Mathf.Min(remaining, cachedSecondaryMaxStock);
            }
        }
    }
}