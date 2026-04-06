using BAMod.Mashiro.Achievements;
using RoR2;
using UnityEngine;

namespace BAMod.Mashiro.Content
{
    public static class MashiroUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                MashiroMasteryAchievements.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(MashiroMasteryAchievements.identifier),
                MashiroSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
