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

namespace BAMod.Arisu.Content
{
    static class ArisuHooks
    {
        static BuffDef BleedDebuff;

        public static void Init()
        {
            BleedDebuff = LegacyResourcesAPI.Load<BuffDef>("RoR2/Base/Common/bdBleeding");
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (!sender) return;
            if (sender.HasBuff(ArisuBuffs.Withstand))
            {
                args.armorAdd += 300;
            }
            if (sender.HasBuff(ArisuBuffs.ArisuOverheatStack))
            {
                args.baseCurseAdd += sender.GetBuffCount(ArisuBuffs.ArisuOverheatStack) * 0.01f;
            }
        }
    }
}
