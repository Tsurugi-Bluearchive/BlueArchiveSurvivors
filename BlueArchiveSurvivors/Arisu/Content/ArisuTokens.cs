using BAMod.Arisu.Achievements;
using BAMod.Modules;
using System;
using UnityEngine.UIElements.Experimental;
using static RoR2.OverheatSystem;

namespace BAMod.Arisu.Content
{
    public static class ArisuTokens
    {
        public static void Init()
        {
            AddHenryTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Henry.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddHenryTokens()
        {
            var prefix = ArisuSurvivor.Arisu_PREFIX;

            var desc = "Henry is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine
             + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine
             + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine
             + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            var outro = "..and so he left, searching for a new identity.";
            var outroFailure = "..and so he vanished, forever a blank slate.";

            Language.Add(prefix + "NAME", "Arisu");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "Shotgun Brawler");
            Language.Add(prefix + "LORE", "sample lore");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "SPECIAL_ULTIMATE_PRIMARY_NAME", "Sword of Light: Hypernova");
            Language.Add(prefix + "SPECIAL_ULTIMATE_PRIMARY_DESCRIPTION", $"Rooting. Armouring. Fire a slowly ramping beam attack that deals <style=cIsDamage>{100f * ArisuStaticValues.ultBeamDamage}% -- {100f * ArisuStaticValues.maxUltBeamDamage}%</style> damage per second. Once out of ammo start Overheating- dealing temporary soul damage to self.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_GUN_NAME", "Sword of Light: Supernova");
            Language.Add(prefix + "PRIMARY_GUN_DESCRIPTION", $"Rooting. Armouring.  Fire a slowly ramping beam attack that deals <style=cIsDamage>{100f * ArisuStaticValues.baseBeamDamage}% -- {100f * ArisuStaticValues.maxBaseBeamDamage}%</style> damage per second. Once out of ammo start Overheating- dealing temporary soul damage to self.");
            #endregion
         
            #region Secondary
            Language.Add(prefix + "SECONDARY_GUN_NAME", "Core Ejection");
            Language.Add(prefix + "SECONDARY_GUN_DESCRIPTION", $"Eject the current fuel core in the gun for {ArisuStaticValues.coreEjectMult * 100}xFuel% Damage");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_STUN_NAME", "Emergency Cooling Solution");
            Language.Add(prefix + "UTILITY_STUN_DESCRIPTION", $"Remove all overheat stacks and shoot a jet of steam out of her backpack, sending her flying and damaging enemies around her for {ArisuStaticValues.coolingMult * 100}xOverheatStack% damage");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_ULTIMATE_NAME", "Awaken, Super Nova!");
            Language.Add(prefix + "SPECIAL_ULTIMATE_DESCRIPTION", $"Rooting. Shielding. Gain 25% of TOTAL HEALTH as shield and remain rooted for 3s while charging, after finished change her primary gun into Sword of Light: Hypernova");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(ArisuMasteryAchievements.identifier), "Henry: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(ArisuMasteryAchievements.identifier), "As Henry, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
