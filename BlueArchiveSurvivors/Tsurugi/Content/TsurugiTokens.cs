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
            Language.Add(prefix + "PRIMARY_GUN_NAME", "Blood");
            Language.Add(prefix + "PRIMARY_GUN_DESCRIPTION", $"Agile. Channeled. Shoot a shotgun blast after holding for half asecond for <style=cIsDamage>30x{100f * TsurugiStaticValues.justiceDamage}%</style> damage with 1.0 Proc every 3 seconds. Starts with 5 rounds and switches to a reload skill once out. Shotgun blasts deal Malice, which hurts enemies for 0.5% HP over 5 seconds per stack</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_GUN_NAME", "Gunpowder");
            Language.Add(prefix + "SECONDARY_GUN_DESCRIPTION", $"Agile. Channeled. Piercing. Shoot a shotgun blast after holding for half asecond for <style=cIsDamage>30x{100f * TsurugiStaticValues.justiceDamage}%</style> damage with 1.0 Proc every 3 seconds. Starts with 5 rounds and switches to a reload skill once out. Shotgun blasts heal for 10% of damage dealt");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_STUN_NAME", "You think you can catch ME?");
            Language.Add(prefix + "UTILITY_STUN_DESCRIPTION", "Etherial. Dodge a large distance while stunning everything that's passed through.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_ULTIMATE_NAME", "Strange and mysterious");
            Language.Add(prefix + "SPECIAL_ULTIMATE_DESCRIPTION", $"Rooting. Sheilding. Implant and gain 25% of TOTAL HEALTH as sheild and.Remain in cast for 3s. Both shotguns now attack for <style=cIsDamage>1000%</style> damage via exploding enemies directly.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(TsurugiMasteryAchievements.identifier), "Henry: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(TsurugiMasteryAchievements.identifier), "As Henry, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
