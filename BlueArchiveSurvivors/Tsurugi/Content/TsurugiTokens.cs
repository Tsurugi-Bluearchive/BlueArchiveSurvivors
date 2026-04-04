using BAMod.Modules;
using BAMod.Tsurugi.Achievements;
using System;

namespace BAMod.Tsurugi.Content
{
    public static class TsurugiTokens
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
            var prefix = TsurugiSurvivor.V1_PREFIX;

            var desc = "Henry is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine
             + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine
             + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine
             + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            var outro = "..and so he left, searching for a new identity.";
            var outroFailure = "..and so he vanished, forever a blank slate.";

            Language.Add(prefix + "NAME", "Tsurugi");
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
            Language.Add(prefix + "PRIMARY_SCARF_NAME", "Scarf");
            Language.Add(prefix + "PRIMARY_SCARF_DESCRIPTION", $"Swing forward for <style=cIsDamage>{100f * TsurugiStaticValues.justiceDamage}% damage</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_GUN_NAME", "Precision Strike");
            Language.Add(prefix + "SECONDARY_GUN_DESCRIPTION", $"IMPLANT. Hold still and fire a picked up gun for <style=cIsDamage>{100f * TsurugiStaticValues.bigGunDamageCefficeient}% damage</style>.");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_ROLL_NAME", "Roll");
            Language.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Roll a short distance, gaining <style=cIsUtility>300 armor</style>. <style=cIsUtility>You cannot be hit during the roll.</style>");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_BOMB_NAME", "Bomb");
            Language.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * TsurugiStaticValues.bigGunDamageCefficeient}% damage</style>.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(TsurugiMasteryAchievements.identifier), "Henry: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(TsurugiMasteryAchievements.identifier), "As Henry, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
