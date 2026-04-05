using BAMod.Tsurugi.Content;
using BAMod.Tsurugi.SkillStates.BaseStates;
using Newtonsoft.Json.Utilities;
using R2API;
using RoR2;
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
            On.RoR2.GlobalEventManager.ServerDamageDealt += GlobalEventManager_ServerDamageDealt;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender)
            {
                if (sender.HasBuff(SaoriBuffs.HyperCritBuff))
                {
                    args.critDamageMultAdd += 5;
                }
                if (sender.HasBuff(SaoriBuffs.SaoriUltShield))
                {
                    sender.baseMaxShield += sender.healthComponent.fullCombinedHealth * 0.1f;
                }
            }
        }

        private static void GlobalEventManager_ServerDamageDealt(On.RoR2.GlobalEventManager.orig_ServerDamageDealt orig, DamageReport damageReport)
        {
            if (damageReport.attackerBody && damageReport.victim && damageReport.attackerBody.HasBuff(SaoriBuffs.HyperCritBuff) && damageReport.victimBody.isBoss)
            {
                damageReport.damageDealt *= 2;
            }
        }
    }
}
