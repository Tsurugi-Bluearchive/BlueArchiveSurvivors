
using BAMod.Tsurugi.SkillStates.Primary;
using BAMod.Tsurugi.SkillStates.Secondary;
using BAMod.Tsurugi.SkillStates.Special;
using UltrakillMod.V1.SkillStates.BaseStates;

namespace BA.Tsurugi.Content
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
