using BAMod.Mutsuki.Content;
using BAMod.Mutsuki.SkillStates.BaseStates;
using R2API;
using RoR2;

namespace BAMod.Mutsuki.SkillStates.Utility
{
    internal class FlameBurst : BaseMutsukiSkillState
    {
        protected override float baseFireDelay => 0f;
        protected override float baseDuration => 2f;
        protected override float fireTime => 0f;

        private bool flamed;
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (!flamed)
                {
                    var flameBlast = new BlastAttack()
                    {
                        damageColorIndex = DamageColorIndex.Default,
                        baseDamage = 0f,
                        attacker = this.gameObject,
                        crit = RollCrit(),
                        position = this.gameObject.transform.position,
                        radius = 40,
                        procCoefficient = 0f,
                    };
                    flameBlast.AddModdedDamageType(MutsukiCustomDamageTypes.MutsukiDoubleIgnite);
                    flameBlast.Fire();
                    flamed = true;
                }
                if (fixedAge > duration)
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
