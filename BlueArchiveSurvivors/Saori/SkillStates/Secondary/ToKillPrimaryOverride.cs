using BAMod.GlobalContent.Components;
using BAMod.Mashiro.Content;
using BAMod.Mashiro.SkillStates.BaseStates;
using BAMod.Saori.Content;
using BAMod.Saori.SkillStates.BaseStates;
using EntityStates.Commando.CommandoWeapon;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BAMod.Saori.SkillStates.Secondary
{
    internal class ToKillPrimaryOverride : BaseSaoriSkillState
    {
        protected override float baseDuration => 2f;
        protected override float baseFireDelay => 0f;
        protected override float fireTime => 0f;

        private GameObject mark;
        public GameObject hitEffectPrefab = FireBarrage.hitEffectPrefab;
        public GameObject tracerEffectPrefab = FireBarrage.tracerEffectPrefab;
        public DamageTypeCombo damageType = DamageType.Generic;
        private bool buffed;
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!isAuthority)
                return;
            if (!buffed)
            {
                var aimRay = GetAimRay();
                var bullseyeSearch = new BullseyeSearch()
                {
                    searchDirection = aimRay.direction,
                    searchOrigin = aimRay.origin,
                    sortMode = BullseyeSearch.SortMode.Angle,
                    maxAngleFilter = 90f,
                    teamMaskFilter = TeamMask.all
                };
                bullseyeSearch.teamMaskFilter.RemoveTeam(this.teamComponent.teamIndex);
                bullseyeSearch.RefreshCandidates();

                if (bullseyeSearch.candidatesEnumerable.Count == 0)
                    return;

                var mainHealthComponent = bullseyeSearch.GetResults().FirstOrDefault().healthComponent;
                mainHealthComponent.body.AddTimedBuff(SaoriBuffs.SaoriPrimaryMarkBuff, 5f, 1);

                var sphereSearch = new SphereSearch()
                {
                    origin = mainHealthComponent.transform.position,
                    radius = 10f,
                    mask = LayerIndex.entityPrecise.mask
                };
                var teamMask = TeamMask.all;
                teamMask.RemoveTeam(this.teamComponent.teamIndex);
                sphereSearch.RefreshCandidates();
                sphereSearch.FilterCandidatesByHurtBoxTeam(teamMask);
                var nearbyHurtboxes = sphereSearch.GetHurtBoxes();



                foreach (var hurtbox in nearbyHurtboxes)
                {
                    if (hurtbox.healthComponent == mainHealthComponent)
                    {
                        continue;
                    }
                    hurtbox.healthComponent.body.AddTimedBuff(SaoriBuffs.SaoriMarkBuff, 5f, 1);
                }
                buffed = true;
            }
            if (buffed && fixedAge > duration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
