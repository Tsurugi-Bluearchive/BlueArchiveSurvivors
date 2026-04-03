using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using R2API;
using UltrakillMod.V1.SkillStates.BaseStates;
using BAMod.Tsurugi.Components;

namespace BA.Tsurugi.Content
{
    static class TsurugiHooks
    {
        static BuffDef BleedDebuff;
        public static void Init()
        {
            BleedDebuff = LegacyResourcesAPI.Load<BuffDef>("RoR2/Base/Common/bdBleeding");
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            On.RoR2.GlobalEventManager.ServerDamageDealt += GlobalEventManager_ServerDamageDealt;
        }

        private static void GlobalEventManager_ServerDamageDealt(On.RoR2.GlobalEventManager.orig_ServerDamageDealt orig, DamageReport damageReport)
        {
            if (damageReport.attacker.gameObject.TryGetComponent<TsurugiCharacterMain>(out var tsurugi))
            {
                if (DamageAPI.HasModdedDamageType(ref damageReport.damageInfo.damageType, TsurugiCustomDamageTypes.BloodBleed))
                {
                    for (int i = 0; i < tsurugi.confirmedPrimaryKills; i++)
                        damageReport.victimBody.AddBuff(BleedDebuff);
                }
                if (DamageAPI.HasModdedDamageType(ref damageReport.damageInfo.damageType, TsurugiCustomDamageTypes.GunpowderHeal))
                {
                    damageReport.attackerBody.healthComponent.Heal((float)tsurugi.confirmedSecondaryKills * (damageReport.damageDealt * 0.1f), new ProcChainMask());
                }
            }
            orig(damageReport);
        }

        private static void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            if (damageReport.attacker.gameObject.TryGetComponent<TsurugiCharacterMain>(out var tsurugi) && self != null)
            {
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
