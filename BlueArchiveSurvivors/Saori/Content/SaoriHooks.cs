using BAMod.GlobalContent.Components;
using BAMod.Mashiro.Content;
using BAMod.Mashiro.SkillStates.BaseStates;
using EntityStates.BrotherMonster;
using Newtonsoft.Json.Utilities;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace BAMod.Saori.Content
{
    static class SaoriHooks
    {
        static BuffDef BleedDebuff;
 
        public static void Init()
        {
            BleedDebuff = LegacyResourcesAPI.Load<BuffDef>("RoR2/Base/Common/bdBleeding");
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterBody.AddBuff_BuffDef += CharacterBody_AddBuff_BuffDef;
            On.RoR2.HealthComponent.Die += HealthComponent_Die;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
           if (self && damageInfo != null && self.body)
            {
                if (self.body.HasBuff(SaoriBuffs.SaoriMarkBuff) || self.body.HasBuff(SaoriBuffs.SaoriPrimaryMarkBuff))
                {
                    var newInfo = new DamageInfo()
                    {
                        damage = damageInfo.damage,
                        damageColorIndex = damageInfo.damageColorIndex,
                        damageType = damageInfo.damageType,
                        delayedDamageSecondHalf = damageInfo.delayedDamageSecondHalf,
                        attacker = damageInfo.attacker,
                        dotIndex = damageInfo.dotIndex,
                        firstHitOfDelayedDamageSecondHalf = damageInfo.firstHitOfDelayedDamageSecondHalf,
                        canRejectForce = damageInfo.canRejectForce,
                        crit = true,
                        force = damageInfo.force,
                        inflictedHurtbox = damageInfo.inflictedHurtbox,
                        inflictor = damageInfo.inflictor,
                        physForceFlags = damageInfo.physForceFlags,
                        position = damageInfo.position,
                        procChainMask = damageInfo.procChainMask,
                        procCoefficient = damageInfo.procCoefficient,
                        rejected = damageInfo.rejected,
                    };
                    orig(self, newInfo);
                    return;
                }

                if (self.body.HasBuff(SaoriBuffs.SaoriPrimaryMarkBuff))
                {
                    var droneSearch = new SphereSearch()
                    {
                        mask = LayerIndex.entityPrecise.mask,
                        origin = self.transform.position,
                        radius = 100f,
                    };

                    droneSearch.RefreshCandidates();
                    var hurtboxes = droneSearch.searchData.GetHurtBoxes();

                    foreach (HurtBox hurtBox in hurtboxes)
                    {
                        if (hurtBox == null || hurtBox.healthComponent == null)
                            continue;

                        CharacterBody droneBody = hurtBox.healthComponent.body;
                        if (droneBody == null)
                            continue;

                        if (droneBody.teamComponent == null || droneBody.teamComponent.teamIndex != TeamIndex.Player)
                            continue;

                        if (droneBody == self.body)
                            continue;

                        CharacterMaster master = droneBody.master;
                        if (master == null)
                            continue;

                        if (master.aiComponents != null && master.aiComponents.Length > 0)
                        {
                            foreach (var ai in master.aiComponents)
                            {
                                if (ai != null)
                                {
                                    ai.currentEnemy = new BaseAI.Target(ai.leader.owner)
                                    {
                                        gameObject = self.gameObject,
                                        healthComponent = self
                                    };

                                    ai.currentEnemy.Update();
                                }
                            }
                        }
                    }
                }
            }
            orig(self, damageInfo);
        }

        private static void HealthComponent_Die(On.RoR2.HealthComponent.orig_Die orig, HealthComponent self, bool noCorpse)
        {
            if (self && self.body && self.body.HasBuff(SaoriBuffs.SaoriPrimaryMarkBuff))
            {
                var transfer = new BullseyeSearch()
                {   
                    maxAngleFilter = 360,
                    maxDistanceFilter = 30,
                    sortMode = BullseyeSearch.SortMode.Distance
                };
                transfer.RefreshCandidates();
                foreach (var transferCanidate in transfer.candidatesEnumerable)
                {
                    if (transferCanidate.hurtBox.healthComponent.body.HasBuff(SaoriBuffs.SaoriMarkBuff))
                    {
                        transferCanidate.hurtBox.healthComponent.body.RemoveBuff(SaoriBuffs.SaoriMarkBuff);
                        transferCanidate.hurtBox.healthComponent.body.AddBuff(SaoriBuffs.SaoriPrimaryMarkBuff);
                        break;
                    }
                }
            }
            orig(self, noCorpse);
        }

        private static void CharacterBody_AddBuff_BuffDef(On.RoR2.CharacterBody.orig_AddBuff_BuffDef orig, CharacterBody self, BuffDef buffDef)
        {
            if (self == null || buffDef == null)
            {
                orig(self, buffDef);
                return;
            }

            if (!self.HasBuff(SaoriBuffs.SaoriPrimaryMarkBuff) && buffDef == SaoriBuffs.SaoriPrimaryMarkBuff)
            {
                var effect = GameObject.Instantiate(SaoriAssets.mainMarkBuffEffect, self.healthComponent ? self.healthComponent.transform : self.transform);
                if (effect != null)
                {
                    var display = effect.GetComponent<DisplayAboveModelTransform>();
                    if (display != null && self.healthComponent != null)
                    {
                        display.victimHealthComponent = self.healthComponent;
                    }
                }
            }

            if (!self.HasBuff(SaoriBuffs.SaoriMarkBuff) && buffDef == SaoriBuffs.SaoriMarkBuff)
            {
                var effect = GameObject.Instantiate(SaoriAssets.secondaryMarkBuffEffect, self.healthComponent ? self.healthComponent.transform : self.transform);
                if (effect != null)
                {
                    var display = effect.GetComponent<DisplayAboveModelTransform>();
                    if (display != null && self.healthComponent != null)
                    {
                        display.victimHealthComponent = self.healthComponent;
                    }
                }
            }

            orig(self, buffDef);
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender)
            {
                if (sender.HasBuff(SaoriBuffs.HyperCritBuff))
                {
                    args.critDamageMultAdd += 5f;
                }
                if (sender.HasBuff(SaoriBuffs.SaoriUltShield))
                {

                    args.baseShieldAdd += sender.healthComponent.fullCombinedHealth * 0.25f;
                }
            }
        }
    }
}
