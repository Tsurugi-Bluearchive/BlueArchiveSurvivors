using R2API;
using RoR2;
using UnityEngine;

namespace BAMod.Tsurugi.Content
{
    public static class SaoriBuffs
    {
        public static BuffDef HyperCritBuff;
        public static BuffDef SaoriUltShield;
        public static void Init()
        {
            HyperCritBuff = Modules.Content.CreateAndAddBuff(
                "Hypercrit",
                LegacyResourcesAPI.Load<Sprite>("RoR2/Base/Bandit2/texBuffBanditSkullIcon"),
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

