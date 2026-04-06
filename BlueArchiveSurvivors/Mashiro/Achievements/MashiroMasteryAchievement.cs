using BAMod.Modules.Achievements;
using RoR2;

namespace BAMod.Mashiro.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION"
   [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    public class MashiroMasteryAchievements : BaseMasteryAchievement
    {
        public const string identifier = MashiroSurvivor.V1_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = MashiroSurvivor.V1_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => MashiroSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}