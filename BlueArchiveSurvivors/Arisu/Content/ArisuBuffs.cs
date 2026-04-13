using R2API;
using RoR2;
using RoR2BepInExPack.GameAssetPaths;
using System.Data.SqlTypes;
using UnityEngine;

namespace BAMod.Arisu.Content
{
    public static class ArisuBuffs
    {
        public static BuffDef Withstand;
        public static BuffDef ArisuUltShield;
        public static BuffDef ArisuOverheatStack;
        public static void Init()
        {
            Withstand = Modules.Content.CreateAndAddBuff(
                "Withstand",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/BanditSkull").iconSprite,
                Color.white,
                false,
                false);

            ArisuUltShield = Modules.Content.CreateAndAddBuff(
            "Ult Shield",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);

            ArisuOverheatStack = Modules.Content.CreateAndAddBuff(
                "Overheat Stack",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/OnFire").iconSprite,
                Color.blue,
                true,
                true);

        }
    }
}

