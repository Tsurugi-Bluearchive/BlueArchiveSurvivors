using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using R2API;
using BAMod.Mashiro.SkillStates.BaseStates;
using BAMod.Mashiro.Components;
using System.Linq;
using Newtonsoft.Json.Utilities;

namespace BAMod.Mashiro.Content
{
    static class MashiroHooks
    {
        static BuffDef BleedDebuff;

        public static void Init()
        {
            BleedDebuff = LegacyResourcesAPI.Load<BuffDef>("RoR2/Base/Common/bdBleeding");
            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void HealthComponent_TakeDamageProcess(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            var echoDamageInfo = new DamageInfo()
            {
                damage = damageInfo.damage,
                damageColorIndex = DamageColorIndex.Sniper,
                inflictedHurtbox = damageInfo.inflictedHurtbox,
                crit = damageInfo.crit,
                damageType = damageInfo.damageType,
                delayedDamageSecondHalf = damageInfo.delayedDamageSecondHalf,
                attacker = damageInfo.attacker,
                canRejectForce = damageInfo.canRejectForce,
                physForceFlags = damageInfo.physForceFlags,
                position = damageInfo.position,
                dotIndex = damageInfo.dotIndex,
                firstHitOfDelayedDamageSecondHalf = damageInfo.firstHitOfDelayedDamageSecondHalf,
                force = damageInfo.force,
                inflictor = damageInfo.inflictor,
                procChainMask = damageInfo.procChainMask,
                procCoefficient = damageInfo.procCoefficient,
                rejected = damageInfo.rejected
            };

            orig(self, damageInfo);
            
            if (self && self.body && damageInfo.inflictedHurtbox && damageInfo.inflictedHurtbox.healthComponent)
            {
                if (self.body.HasBuff(MashiroBuffs.AttackEcho) && !damageInfo.HasModdedDamageType(MashiroCustomDamageTypes.EchoDamage))
                {
                    echoDamageInfo.AddModdedDamageType(MashiroCustomDamageTypes.EchoDamage);
                    damageInfo.inflictedHurtbox.healthComponent.TakeDamage(echoDamageInfo);
                }
            }
        }
        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender)
            {
                if (sender.HasBuff(MashiroBuffs.MashiroUltShield))
                {
                    args.baseShieldAdd += sender.healthComponent.fullCombinedHealth * 0.25f;
                }
            }
        }
    }
}
