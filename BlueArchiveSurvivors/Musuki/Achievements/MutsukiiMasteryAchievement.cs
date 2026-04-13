using BAMod.Modules.Achievements;
using RoR2;

namespace BAMod.Mutsuki.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION"
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    public class MutsukiMasteryAchievements : BaseMasteryAchievement
    {
        public const string identifier = MutsukiSurvivor.Mutsuki_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = MutsukiSurvivor.Mutsuki_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => MutsukiSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}