using R2API;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAMod.Mashiro.Content
{
    internal class MashiroCustomDamageTypes
    {
        /// <summary>
        /// The modded damage type to track if attacks are an echo.
        /// </summary>
        public static DamageAPI.ModdedDamageType EchoDamage = DamageAPI.ReserveDamageType();
    }
}