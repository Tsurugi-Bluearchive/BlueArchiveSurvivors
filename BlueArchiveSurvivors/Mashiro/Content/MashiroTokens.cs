using BAMod.Modules;
using BAMod.Mashiro.Achievements;
using System;

namespace BAMod.Mashiro.Content
{
    public static class MashiroTokens
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
            var prefix = MashiroSurvivor.V1_PREFIX;

            var desc = "Henry is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine
             + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine
             + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine
             + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            var outro = "..and so he left, searching for a new identity.";
            var outroFailure = "..and so he vanished, forever a blank slate.";

            Language.Add(prefix + "NAME", "Mashiro");
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
            Language.Add(prefix + "PRIMARY_GUN_NAME", "Revelation of Justice");
            Language.Add(prefix + "PRIMARY_GUN_DESCRIPTION", $"Agile.Piercing. Fire a small snapsot tank round for <style=cIsDamage>{100f * MashiroStaticValues.smallGunDamageCoefficient}%</style> damage with 1.0 Proc every 1 second.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_SCOPE_NAME", "Tank Round");
            Language.Add(prefix + "SECONDARY_SCOPE_DESCRIPTION", $"Agile. Reecoil. Piercing. Fire a tank round for <style=cIsDamage>{100f * MashiroStaticValues.bigGunDamageCefficeient}%</style> damage with 3.0 proc that explodes for the same amount of damage, again, when contacing the ground. This action causes imense recoil that sends you flying backwards.");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_FLY_NAME", "Wind Burst?");
            Language.Add(prefix + "UTILITY_FLY_DESCRIPTION", "Boost up into the sky and take flight");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_ULTIMATE_NAME", "Shot of Justice");
            Language.Add(prefix + "SPECIAL_ULTIMATE_DESCRIPTION", $"Rooting. Sheilding. Implant and gain 25% of TOTAL HEALTH as sheild and.Remain in cast for All damage now echos an additional time for 10s");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(MashiroMasteryAchievements.identifier), "Henry: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(MashiroMasteryAchievements.identifier), "As Henry, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
