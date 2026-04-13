using BAMod.Modules.Achievements;
using RoR2;

namespace BAMod.Momoi.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION"
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    public class MomoiMasteryAchievements : BaseMasteryAchievement
    {
        public const string identifier = MomoiSurvivor.Momoi_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = MomoiSurvivor.Momoi_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => MomoiSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}