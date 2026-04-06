using BAMod.Mashiro.Content;
using BAMod.Mashiro.SkillStates.BaseStates;
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
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender)
            {
                if (sender.HasBuff(SaoriBuffs.HyperCritBuff))
                {
                    args.critDamageMultAdd += 2.5f;
                    args.critAdd += 100;
                }
                if (sender.HasBuff(SaoriBuffs.SaoriUltShield))
                {

                    args.baseShieldAdd += sender.healthComponent.fullCombinedHealth * 0.25f;
                }
            }
        }
    }
}
