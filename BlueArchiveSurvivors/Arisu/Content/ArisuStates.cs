
using BAMod.Arisu.SkillStates.BaseStates;
using BAMod.Arisu.SkillStates.Primary;
using BAMod.Arisu.SkillStates.Secondary;
using BAMod.Arisu.SkillStates.Special;
using BAMod.Mashiro.SkillStates.BaseStates;
namespace BAMod.Arisu.Content
{
    public static class ArisuStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(ArisuCharacterMain));
            Modules.Content.AddEntityState(typeof(CoreEject));
            Modules.Content.AddEntityState(typeof(BeamAttack));
            Modules.Content.AddEntityState(typeof(BeamAttackOverheat));
            Modules.Content.AddEntityState(typeof(ArisuUlt));
            Modules.Content.AddEntityState(typeof(ArisuUltBeamAttack));
            Modules.Content.AddEntityState(typeof(ArisuUltBeamAttackOverheat));
        }
    }
}
