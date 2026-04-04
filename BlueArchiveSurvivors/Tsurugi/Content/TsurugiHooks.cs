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
            On.RoR2.GlobalEventManager.ServerDamageDealt += GlobalEventManager_ServerDamageDealt;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender != null && args != null)
            {
                if (EntityStateMachine.TryFindByCustomName(sender.gameObject, "Body", out var tsurugiStateMachine) && tsurugiStateMachine.state.GetType() == typeof(TsurugiCharacterMain))
                {
                    args.primarySkill.bonusStockAdd += sender.skillLocator.secondary.maxStock - 5;
                }
            }
        }
        private static void GlobalEventManager_ServerDamageDealt(On.RoR2.GlobalEventManager.orig_ServerDamageDealt orig, DamageReport damageReport)
        {
            if (damageReport.victim && damageReport.attacker && EntityStateMachine.TryFindByCustomName(damageReport.attackerBody.gameObject, "Body", out var tsurugiStateMachine) && tsurugiStateMachine.state.GetType() == typeof(TsurugiCharacterMain))
            {
                var tsurugi = (TsurugiCharacterMain)tsurugiStateMachine.state;
                if (DamageAPI.HasModdedDamageType(ref damageReport.damageInfo.damageType, TsurugiCustomDamageTypes.BloodBleed))
                {
                    var MaliceInfliction = new InflictDotInfo()
                    {
                        dotIndex = TsurugiBuffs.Malice,
                        duration = 5,
                        maxStacksFromAttacker = 40,
                        damageMultiplier = 1,
                        totalDamage = damageReport.victim.combinedHealth / 0.5f,
                        victimObject = damageReport.victimBody.gameObject,
                        attackerObject = damageReport.attackerBody.gameObject,
                        hitHurtBox = damageReport.victimBody.mainHurtBox
                    };
                    DotController.InflictDot(ref MaliceInfliction);
                }
                if (DamageAPI.HasModdedDamageType(ref damageReport.damageInfo.damageType, TsurugiCustomDamageTypes.GunpowderHeal))
                {
                    damageReport.attackerBody.healthComponent.Heal(((float)tsurugi.confirmedSecondaryKills * (damageReport.damageDealt * 0.1f)) + (damageReport.damageDealt * 0.1f), new ProcChainMask());
                }
            }
            orig(damageReport);
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
