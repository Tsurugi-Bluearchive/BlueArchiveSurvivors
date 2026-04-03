using BA.Modules.Achievements;
using RoR2;

namespace BA.Tsurugi.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION"
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    public class TsurugiMasteryAchievements : BaseMasteryAchievement
    {
        public const string identifier = TsurugiSurvivor.V1_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = TsurugiSurvivor.V1_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => TsurugiSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}