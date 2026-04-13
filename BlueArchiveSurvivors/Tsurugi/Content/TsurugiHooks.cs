using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using R2API;
using BAMod.Tsurugi.SkillStates.BaseStates;
using BAMod.Tsurugi.Components;
using System.Linq;
using Newtonsoft.Json.Utilities;

namespace BAMod.Tsurugi.Content
{
    static class TsurugiHooks
    {
        static BuffDef BleedDebuff;
 
        public static void Init()
        {
            BleedDebuff = LegacyResourcesAPI.Load<BuffDef>("RoR2/Base/Common/bdBleeding");
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void GlobalEventManager_onServerDamageDealt(DamageReport obj)
        {
            if (obj.victim && obj.attacker && obj.attackerBody  && EntityStateMachine.TryFindByCustomName(obj.attackerBody.gameObject, "Body", out var tsurugiStateMachine) && tsurugiStateMachine.state.GetType() == typeof(TsurugiCharacterMain))
            {
                var tsurugi = (TsurugiCharacterMain)tsurugiStateMachine.state;
                if (DamageAPI.HasModdedDamageType(ref obj.damageInfo.damageType, TsurugiCustomDamageTypes.BloodBleed))
                {
                    var MaliceInfliction = new InflictDotInfo()
                    {
                        dotIndex = TsurugiBuffs.Malice,
                        duration = 5,
                        maxStacksFromAttacker = 40,
                        damageMultiplier = 1,
                        totalDamage = obj.victim.combinedHealth / 0.5f,
                        victimObject = obj.victimBody.gameObject,
                        attackerObject = obj.attackerBody.gameObject,
                        hitHurtBox = obj.victimBody.mainHurtBox
                    };
                    DotController.InflictDot(ref MaliceInfliction);
                }
                if (DamageAPI.HasModdedDamageType(ref obj.damageInfo.damageType, TsurugiCustomDamageTypes.GunpowderHeal))
                {
                    tsurugi.HealBy += obj.damageDealt * 0.1f;
                }
            }
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender != null && args != null)
            {
                if (EntityStateMachine.TryFindByCustomName(sender.gameObject, "Body", out var tsurugiStateMachine) && tsurugiStateMachine.state.GetType() == typeof(TsurugiCharacterMain))
                {
                    args.primarySkill.bonusStockAdd += sender.skillLocator.secondary.maxStock - 5;
                }
                if (sender.HasBuff(TsurugiBuffs.TsurugiUltShield))
                {
                    args.baseShieldAdd += sender.healthComponent.fullCombinedHealth * 0.25f;
                }
            }
        }
        private static void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            if (damageReport.victim && damageReport.attacker && EntityStateMachine.TryFindByCustomName(damageReport.attackerBody.gameObject, "Body", out var tsurugiStateMachine) && self != null && tsurugiStateMachine.state.GetType() == typeof(TsurugiCharacterMain))
            {
                var tsurugi = (TsurugiCharacterMain)tsurugiStateMachine.state;
                if (DamageAPI.HasModdedDamageType(ref damageReport.damageInfo.damageType, TsurugiCustomDamageTypes.BloodBleed))
                {
                    tsurugi.confirmedPrimaryKills += 1;
                }
                if (DamageAPI.HasModdedDamageType(ref damageReport.damageInfo.damageType, TsurugiCustomDamageTypes.GunpowderHeal))
                {
                    tsurugi.confirmedSecondaryKills += 1;
                }
                if (damageReport.attacker.gameObject.TryGetComponent<CrazyWomanPassiveDictator>(out var tsurugiPassive))
                {
                    var passive = tsurugiPassive.GetPassiveType();
                    if (passive == CrazyWomanPassiveDictator.TsurugiPassive.BloodLust)
                    {
                        tsurugi.primaryCritRack = true;
                        tsurugi.secondaryCritRack = true;
                    }
                    else if (passive == CrazyWomanPassiveDictator.TsurugiPassive.BloodSewing)
                    {
                        tsurugi.resetStocks = true;
                    }
                }
            }
            orig(self, damageReport);
        }

    }
}
