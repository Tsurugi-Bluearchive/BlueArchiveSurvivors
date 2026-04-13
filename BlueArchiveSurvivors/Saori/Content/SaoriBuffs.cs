using R2API;
using RoR2;
using System.Data.SqlTypes;
using UnityEngine;

namespace BAMod.Saori.Content
{
    public static class SaoriBuffs
    {
        public static BuffDef HyperCritBuff;
        public static BuffDef SaoriUltShield;
        public static BuffDef SaoriPrimaryMarkBuff;
        public static BuffDef SaoriMarkBuff;
        public static BuffDef SaoriCommanding;
        public static void Init()
        {
            HyperCritBuff = Modules.Content.CreateAndAddBuff(
                "Hypercrit",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/BanditSkull").iconSprite,
                Color.white,
                false,
                false);

            SaoriUltShield = Modules.Content.CreateAndAddBuff(
            "Ult Shield",
            LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
            Color.white,
            false,
            false);

            SaoriPrimaryMarkBuff = Modules.Content.CreateAndAddBuff(
            "Primary Mark",
            LegacyResourcesAPI.Load<BuffDef>("BuffDefs/FullCrit").iconSprite,
            Color.blue,
            false,
            true);

            SaoriMarkBuff = Modules.Content.CreateAndAddBuff(
            "Secondary Mark",
            LegacyResourcesAPI.Load<BuffDef>("BuffDefs/FullCrit").iconSprite,
            Color.cyan,
            false,
            true);

            SaoriCommanding = Modules.Content.CreateAndAddBuff(
            "Drone Command",
            LegacyResourcesAPI.Load<BuffDef>("BuffDefs/FullCrit").iconSprite,
            Color.cyan,
            false,
            false);
        }
    }
}

