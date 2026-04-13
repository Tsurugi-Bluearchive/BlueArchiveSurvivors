using BAMod.Momoi.Achievements;
using RoR2;
using UnityEngine;

namespace BAMod.Momoi.Content
{
    public static class MomoiUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                MomoiMasteryAchievements.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(MomoiMasteryAchievements.identifier),
                MomoiSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
