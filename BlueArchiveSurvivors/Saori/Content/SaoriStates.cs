
using BAMod.Saori.SkillStates.Primary;
using BAMod.Saori.SkillStates.Secondary;
using BAMod.Saori.SkillStates.Special;
using BAMod.Tsurugi.SkillStates.BaseStates;
namespace BAMod.Saori.Content
{
    public static class SaoriStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(SaoriCharacterMain));
            Modules.Content.AddEntityState(typeof(Scope));
            Modules.Content.AddEntityState(typeof(ScopePrimaryOverride));
            Modules.Content.AddEntityState(typeof(BurstRifle));
            Modules.Content.AddEntityState(typeof(BurstRifleReload));
            Modules.Content.AddEntityState(typeof(SaoriUlt));
        }
    }
}
