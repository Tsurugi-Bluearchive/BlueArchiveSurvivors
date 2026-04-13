using R2API;
using RoR2;
using System.Data.SqlTypes;
using UnityEngine;

namespace BAMod.Mutsuki.Content
{
    public static class MutsukiBuffs
    {
        public static BuffDef HyperCritBuff;
        public static BuffDef MutsukiUltShield;
        public static void Init()
        {
            HyperCritBuff = Modules.Content.CreateAndAddBuff(
                "Hypercrit",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/BanditSkull").iconSprite,
                Color.white,
                false,
                false);

            MutsukiUltShield = Modules.Content.CreateAndAddBuff(
            "Ult Shield",
            LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
            Color.white,
            false,
            false);
        }
    }
}

