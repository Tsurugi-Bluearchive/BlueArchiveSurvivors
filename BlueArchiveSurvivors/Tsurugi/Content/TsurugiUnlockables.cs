using BA.Tsurugi.Achievements;
using RoR2;
using UnityEngine;

namespace BA.Tsurugi.Content
{
    public static class TsurugiUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                TsurugiMasteryAchievements.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(TsurugiMasteryAchievements.identifier),
                TsurugiSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
