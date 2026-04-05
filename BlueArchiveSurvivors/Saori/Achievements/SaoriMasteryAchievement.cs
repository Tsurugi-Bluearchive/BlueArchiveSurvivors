using BAMod.Modules.Achievements;
using RoR2;

namespace BAMod.Saori.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION"
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    public class SaoriMasteryAchievements : BaseMasteryAchievement
    {
        public const string identifier = SaoriSurvivor.SAORI_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = SaoriSurvivor.SAORI_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => SaoriSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}