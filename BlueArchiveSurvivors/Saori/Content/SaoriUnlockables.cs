using BAMod.Saori.Achievements;
using RoR2;
using UnityEngine;

namespace BAMod.Saori.Content
{
    public static class SaoriUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                SaoriMasteryAchievements.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(SaoriMasteryAchievements.identifier),
                SaoriSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
