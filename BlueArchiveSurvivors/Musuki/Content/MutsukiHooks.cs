using BAMod.GlobalContent.Components;
using BAMod.Mashiro.Content;
using BAMod.Mashiro.SkillStates.BaseStates;
using BAMod.Saori.Content;
using EntityStates.BrotherMonster;
using Newtonsoft.Json.Utilities;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace BAMod.Mutsuki.Content
{
    static class MutsukiHooks
    {
        static BuffDef BleedDebuff;

        public static void Init()
        {
            BleedDebuff = LegacyResourcesAPI.Load<BuffDef>("RoR2/Base/Common/bdBleeding");
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(MutsukiBuffs.MutsukiUltShield))
            {
                args.baseShieldAdd += sender.healthComponent.fullCombinedHealth * 0.25f;
            }
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (!self || damageInfo == null)
            {
                orig(self, damageInfo);
                return;
            }
            if (self.body && damageInfo.HasModdedDamageType(MutsukiCustomDamageTypes.MutsukiIgnite))
            {
                InflictDotInfo dotInfo = new InflictDotInfo()
                {
                    attackerObject = damageInfo.attacker,
                    damageMultiplier = 1f,
                    dotIndex = DotController.DotIndex.Burn,
                    duration = 10f,
                    totalDamage = damageInfo.damage * 10,
                    hitHurtBox = damageInfo.inflictedHurtbox,
                    preUpgradeDotIndex = DotController.DotIndex.Burn,
                    victimObject = self.gameObject,
                };
                DotController.InflictDot(ref dotInfo);
            }
            if (self.body && damageInfo.HasModdedDamageType(MutsukiCustomDamageTypes.MutsukiFlameGrenade))
            {
                InflictDotInfo dotInfo = new InflictDotInfo()
                {
                    attackerObject = damageInfo.attacker,
                    damageMultiplier = 1f,
                    dotIndex = DotController.DotIndex.Burn,
                    duration = 10f,
                    totalDamage = damageInfo.damage * 10,
                    hitHurtBox = damageInfo.inflictedHurtbox,
                    preUpgradeDotIndex = DotController.DotIndex.Burn,
                    victimObject = self.gameObject,
                };
                for (int i = 0; i < 10; i++)
                {
                    DotController.InflictDot(ref dotInfo);
                }
            }
            if (self.body && damageInfo.HasModdedDamageType(MutsukiCustomDamageTypes.MutsukiDoubleIgnite))
            {
                var flames = self.body.GetBuffCount(RoR2Content.Buffs.OnFire);
                var Dot = new InflictDotInfo()
                {
                    dotIndex = DotController.DotIndex.Burn,
                    damageMultiplier = 1f,
                    duration = 10f,
                    totalDamage = damageInfo.damage,
                    attackerObject = damageInfo.attacker,
                    hitHurtBox = damageInfo.inflictedHurtbox,
                    victimObject = self.body.gameObject
                };
                if (damageInfo.crit)
                {
                    flames *= 3;
                }
                for (int i = 0; i < flames; i++)
                {
                    DotController.InflictDot(ref Dot);
                }
            }
            orig(self, damageInfo);
        }
    }
}
