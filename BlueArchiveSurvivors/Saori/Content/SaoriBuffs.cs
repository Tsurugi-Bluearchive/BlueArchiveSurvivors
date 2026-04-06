using R2API;
using RoR2;
using UnityEngine;

namespace BAMod.Mashiro.Content
{
    public static class SaoriBuffs
    {
        public static BuffDef HyperCritBuff;
        public static BuffDef SaoriUltShield;
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
        }
    }
}

