using BAMod.Modules.Achievements;
using RoR2;

namespace BAMod.Arisu.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION"
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    public class ArisuMasteryAchievements : BaseMasteryAchievement
    {
        public const string identifier = ArisuSurvivor.Arisu_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = ArisuSurvivor.Arisu_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => ArisuSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}