using BAMod.GlobalContent.Components;
using BAMod.Saori.Content;
using BAMod.Saori.SkillStates.BaseStates;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using R2API;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BAMod.Saori.SkillStates.Secondary
{
    internal class ToKill : BaseSaoriSkillState
    {
        protected override float baseDuration => 4;
        protected override float baseFireDelay => 0.5f;
        protected override float fireTime => 1;
        private bool fired = false;
        public GameObject hitEffectPrefab = FireBarrage.hitEffectPrefab;
        public GameObject tracerEffectPrefab = FireBarrage.tracerEffectPrefab;
        public DamageTypeCombo damageType = DamageType.Generic;
        private GameObject mainMark;
        private List<DisplayAboveModelTransform> subMarkTransform = new();

        public override void OnEnter()
        {
            base.OnEnter();
            skillLocator.primary.SetSkillOverride(this.gameObject, SaoriSurvivor.ScopePrimaryOverride, GenericSkill.SkillOverridePriority.Default);
            skillLocator.primary.OverrideMaxStock(skillLocator.secondary.stock);
            skillLocator.primary.stock = skillLocator.primary.maxStock;
            skillLocator.secondary.stock = 0;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!isAuthority)
                return;

            if (!IsKeyDownAuthority())
            {
                outer.SetNextStateToMain();
                return;
            }

            var aimRay = GetAimRay();

            var search = new BullseyeSearch()
            {
                searchDirection = aimRay.direction,
                searchOrigin = aimRay.origin,
                sortMode = BullseyeSearch.SortMode.Angle,
                maxAngleFilter = 90f,
                teamMaskFilter = TeamMask.all
            };
            search.teamMaskFilter.RemoveTeam(this.teamComponent.teamIndex);
            search.RefreshCandidates();

            if (search.candidatesEnumerable.Count == 0)
                return;

            var mainHurtbox = search.GetResults().FirstOrDefault();
            var mainHealthComponent = mainHurtbox.healthComponent;

            if (!mainMark)
            {
                mainMark = GameObject.Instantiate(SaoriAssets.markTargetEffect);
            }
            var mainDisplay = mainMark.GetComponent<DisplayAboveModelTransform>();
            mainDisplay.victimHealthComponent = mainHealthComponent;

            var surroundingsearch = new SphereSearch()
            {
                origin = mainHurtbox.transform.position,
                radius = 10f,
                mask = LayerIndex.entityPrecise.mask
            };
            surroundingsearch.RefreshCandidates();
            var teamMask = TeamMask.all;
            teamMask.RemoveTeam(this.teamComponent.teamIndex);
            surroundingsearch.FilterCandidatesByHurtBoxTeam(teamMask);
            var nearbyHurtboxes = surroundingsearch.GetHurtBoxes();

            for (int i = subMarkTransform.Count - 1; i >= 0; i--)
            {
                var mark = subMarkTransform[i];

                bool stillNearby = false;
                foreach (var hurtbox in nearbyHurtboxes)
                {
                    if (hurtbox.healthComponent == mark.victimHealthComponent)
                    {
                        stillNearby = true;
                        break;
                    }
                }

                if (!stillNearby)
                {
                    if (mark.gameObject != null)
                        Destroy(mark.gameObject);

                    subMarkTransform.RemoveAt(i);
                }
            }

            foreach (var hurtbox in nearbyHurtboxes)
            {
                if (hurtbox.healthComponent == mainHealthComponent)
                    continue;

                bool alreadyMarked = false;
                foreach (var mark in subMarkTransform)
                {
                    if (mark.victimHealthComponent == hurtbox.healthComponent)
                    {
                        alreadyMarked = true;
                        break;
                    }
                }

                if (!alreadyMarked)
                {
                    var newMarkObj = GameObject.Instantiate(SaoriAssets.markSecondaryTargetEffect);
                    var display = newMarkObj.GetComponent<DisplayAboveModelTransform>();
                    display.victimHealthComponent = hurtbox.healthComponent;

                    subMarkTransform.Add(display);
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (mainMark) Destroy(mainMark);

            foreach (var mark in subMarkTransform)
            {
                if (mark != null && mark.gameObject != null)
                    Destroy(mark.gameObject);
            }
            subMarkTransform.Clear();

            skillLocator.secondary.stock = skillLocator.primary.stock > 0 ? Mathf.RoundToInt(skillLocator.primary.stock) : 0;
            skillLocator.primary.UnsetSkillOverride(this.gameObject, SaoriSurvivor.ScopePrimaryOverride, GenericSkill.SkillOverridePriority.Default);
            if (skillLocator.secondary.stock <= 0)
            {
                skillLocator.primary.SetSkillOverride(this.gameObject, SaoriSurvivor.BurstRifleReload, GenericSkill.SkillOverridePriority.Default);
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
