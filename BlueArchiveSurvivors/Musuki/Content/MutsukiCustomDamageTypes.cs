using R2API;
using RoR2BepInExPack.GameAssetPaths;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAMod.Mutsuki.Content
{
    internal class MutsukiCustomDamageTypes
    {
        public static DamageAPI.ModdedDamageType MutsukiIgnite = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType MutsukiDoubleIgnite = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType MutsukiFlameGrenade = DamageAPI.ReserveDamageType();
    }
}
