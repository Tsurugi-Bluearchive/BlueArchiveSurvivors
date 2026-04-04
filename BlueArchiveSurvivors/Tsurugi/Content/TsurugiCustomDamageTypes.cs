using R2API;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAMod.Tsurugi.Content
{
    internal class TsurugiCustomDamageTypes
    {
        /// <summary>
        /// Justice bleed damage type
        /// </summary>
        public static DamageAPI.ModdedDamageType BloodBleed = DamageAPI.ReserveDamageType();

        public static DamageAPI.ModdedDamageType GunpowderHeal = DamageAPI.ReserveDamageType(); 
    }
}
