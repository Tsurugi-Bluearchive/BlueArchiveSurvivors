
using BAMod.Mashiro.SkillStates.BaseStates;
using BAMod.Mashiro.SkillStates.Primary;
using BAMod.Mashiro.SkillStates.Secondary;
using BAMod.Mashiro.SkillStates.Special;
using BAMod.Mashiro.SkillStates.Utility;
namespace BAMod.Mashiro.Content
{
    public static class MashiroStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(MashiroCharacterMain));
            Modules.Content.AddEntityState(typeof(BigRound));
            Modules.Content.AddEntityState(typeof(BigRoundReload));
            Modules.Content.AddEntityState(typeof(Snapshot));
            Modules.Content.AddEntityState(typeof(Scope));
            Modules.Content.AddEntityState(typeof(TakeFlight));
            Modules.Content.AddEntityState(typeof(MashiroUlt));
        }
    }
}
