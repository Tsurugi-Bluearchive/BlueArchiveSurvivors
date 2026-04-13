
using BAMod.Momoi.SkillStates.Primary;
using BAMod.Momoi.SkillStates.Secondary;
using BAMod.Momoi.SkillStates.Special;
using BAMod.Mashiro.SkillStates.BaseStates;
namespace BAMod.Momoi.Content
{
    public static class MomoiStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(MomoiCharacterMain));
            Modules.Content.AddEntityState(typeof(FlameGrenade));
            Modules.Content.AddEntityState(typeof(FlameRifle));
            Modules.Content.AddEntityState(typeof(MomoiUlt));
        }
    }
}
