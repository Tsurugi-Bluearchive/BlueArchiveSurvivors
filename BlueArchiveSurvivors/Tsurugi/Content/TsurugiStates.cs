
using BAMod.Tsurugi.SkillStates.BaseStates;
using BAMod.Tsurugi.SkillStates.Primary;
using BAMod.Tsurugi.SkillStates.Secondary;
using BAMod.Tsurugi.SkillStates.Special;
namespace BAMod.Tsurugi.Content
{
    public static class TsurugiStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(TsurugiCharacterMain));
            Modules.Content.AddEntityState(typeof(Gunpowder));
            Modules.Content.AddEntityState(typeof(GunpowderReload));
            Modules.Content.AddEntityState(typeof(Blood));
            Modules.Content.AddEntityState(typeof(BloodReload));
            Modules.Content.AddEntityState(typeof(TsurugiUlt));
        }
    }
}
