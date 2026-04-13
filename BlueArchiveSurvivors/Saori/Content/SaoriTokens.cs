using BAMod.Modules;
using BAMod.Saori.Achievements;
using System;

namespace BAMod.Saori.Content
{
    public static class SaoriTokens
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
            var prefix = SaoriSurvivor.SAORI_PREFIX;

            var desc = "Henry is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine
             + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine
             + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine
             + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            var outro = "..and so he left, searching for a new identity.";
            var outroFailure = "..and so he vanished, forever a blank slate.";

            Language.Add(prefix + "NAME", "Saori");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "Shotgun Brawler");
            Language.Add(prefix + "LORE", "sample lore");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Henry passive");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "Sample text.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_GUN_NAME", "Arius assault rifle");
            Language.Add(prefix + "PRIMARY_GUN_DESCRIPTION", $"Repeating. <style=cIsDamage>6x{100f * SaoriStaticValues.burstDamage}%</style> damage with 1.0 Proc every 1 seconds. Starts with 5 rounds and switches to a reload skill once out.</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_GUN_NAME", "Scope In");
            Language.Add(prefix + "SECONDARY_GUN_DESCRIPTION", $"Turns primary into 'To Kill Them', which marks targets, gaurinteeing critical strike damage and drone targeting priority");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_STUN_NAME", "Horah!");
            Language.Add(prefix + "UTILITY_STUN_DESCRIPTION", "Etherial. Dodge a large distance while stunning everything that's passed through.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_ULTIMATE_NAME", "Et Omnia Vnitas！  ...and all is vanity!");
            Language.Add(prefix + "SPECIAL_ULTIMATE_DESCRIPTION", $"Rooting. Shielding. Implant and Gain of TOTAL HEALTH as shield and remain rooted for 3s while charging, after finished allies now have 500% additional crit damage for 20s");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(SaoriMasteryAchievements.identifier), "Henry: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(SaoriMasteryAchievements.identifier), "As Henry, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
