using R2API;
using RoR2BepInExPack.GameAssetPaths;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAMod.Momoi.Content
{
    internal class MomoiCustomDamageTypes
    {
        public static DamageAPI.ModdedDamageType MomoiIgnite = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType MomoiDoubleIgnite = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType MomoiFlameGrenade = DamageAPI.ReserveDamageType();
    }
}
