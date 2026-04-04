using R2API;
using RoR2;
using UnityEngine;

namespace BAMod.Tsurugi.Content
{
    public static class TsurugiBuffs
    {
        public static BuffDef MaliceDef;
        public static DotController.DotIndex Malice;

        public static void Init()
        {
            MaliceDef = new BuffDef()
            {
                buffColor = Color.white,
                canStack = true,
                isDOT = true,
                isDebuff = true,
                name = "Malice",
                iconSprite = TsurugiAssets.Malice,
                isHidden = false,
            };
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

