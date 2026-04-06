using R2API;
using RoR2;
using UnityEngine;

namespace BAMod.Mashiro.Content
{
    public static class MashiroBuffs
    {
        public static BuffDef MashiroUltShield;
        public static BuffDef AttackEcho;

        public static void Init()
        {
            AttackEcho = Modules.Content.CreateAndAddBuff(
                "Attack Echo",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/FullCrit").iconSprite,
                Color.white,
                true,
                true);

            MashiroUltShield = Modules.Content.CreateAndAddBuff(
                "Ult Shield",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);
        }
    }
}

