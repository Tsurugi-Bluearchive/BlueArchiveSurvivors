using BAMod.Mutsuki.Achievements;
using RoR2;
using UnityEngine;

namespace BAMod.Mutsuki.Content
{
    public static class MutsukiUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                MutsukiMasteryAchievements.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(MutsukiMasteryAchievements.identifier),
                MutsukiSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
