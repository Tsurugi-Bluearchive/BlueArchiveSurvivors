using R2API;
using RoR2;
using System.Data.SqlTypes;
using UnityEngine;

namespace BAMod.Momoi.Content
{
    public static class MomoiBuffs
    {
        public static BuffDef HyperCritBuff;
        public static BuffDef MomoiUltShield;
        public static void Init()
        {
            HyperCritBuff = Modules.Content.CreateAndAddBuff(
                "Hypercrit",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/BanditSkull").iconSprite,
                Color.white,
                false,
                false);

            MomoiUltShield = Modules.Content.CreateAndAddBuff(
            "Ult Shield",
            LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
            Color.white,
            false,
            false);
        }
    }
}

