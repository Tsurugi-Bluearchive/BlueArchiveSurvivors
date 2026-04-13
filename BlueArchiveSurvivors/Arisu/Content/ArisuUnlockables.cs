using BAMod.Arisu.Achievements;
using RoR2;
using UnityEngine;

namespace BAMod.Arisu.Content
{
    public static class ArisuUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                ArisuMasteryAchievements.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(ArisuMasteryAchievements.identifier),
                ArisuSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
