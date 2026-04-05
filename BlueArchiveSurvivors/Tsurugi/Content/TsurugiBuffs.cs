using R2API;
using RoR2;
using UnityEngine;

namespace BAMod.Tsurugi.Content
{
    public static class TsurugiBuffs
    {
        public static BuffDef TsurugiUltShield;
        public static BuffDef MaliceDef;
        public static DotController.DotIndex Malice;

        public static void Init()
        {
            MaliceDef = Modules.Content.CreateAndAddBuff(
                "Malice",
                TsurugiAssets.Malice,
                Color.white,
                true,
                true);

            TsurugiUltShield = Modules.Content.CreateAndAddBuff(
                "Ult Shield",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);

            Malice = DotAPI.RegisterDotDef(new DotController.DotDef()
            {
                associatedBuff = MaliceDef,
                damageCoefficient = 0,
                interval = 0.5f,
                resetTimerOnAdd = true,
                terminalTimedBuffDuration = 2,
                damageColorIndex = DamageColorIndex.Bleed
            });
        }
    }
}

