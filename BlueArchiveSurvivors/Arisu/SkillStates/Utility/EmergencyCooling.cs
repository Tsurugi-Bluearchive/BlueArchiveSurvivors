using BAMod.Arisu.Content;
using BAMod.Arisu.SkillStates.BaseStates;
using R2API;
using RoR2;
using UnityEngine;

namespace BAMod.Arisu.SkillStates.Utility
{
    internal class EmergencyCooling : BaseArisuSkillState
    {
        protected override float baseFireDelay => 0f;
        protected override float baseDuration => 0.5f;
        protected override float fireTime => 0f;

        private bool cooled;
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isAuthority)
            {
                characterMotor.velocity = Vector3.zero;
                characterMotor.rootMotion += GetMoveVector();
                if (!cooled)
                {
                    var toCool = this.characterBody.GetBuffCount(ArisuBuffs.ArisuOverheatStack);
                    for (int i = 0; i < toCool; i++)
                    {
                        this.characterBody.RemoveBuff(ArisuBuffs.ArisuOverheatStack);
                    }
                    new BlastAttack()
                    {
                        radius = 10f,
                        crit = RollCrit(),
                        baseDamage = toCool * ArisuStaticValues.coolingMult * damageStat,
                        damageColorIndex = DamageColorIndex.Electrocution,
                        position = this.gameObject.transform.position,
                        teamIndex = this.teamComponent.teamIndex,
                        procCoefficient = 1.0f,
                        damageType = DamageType.AOE,
                        attacker = this.gameObject
                    }.Fire();
                    cooled = true;
                }
                if (fixedAge > duration)
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }
        private Vector3 GetMoveVector()
        {
            Vector3 moveVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            moveVector = moveVector * (5 * moveSpeedStat * GetDeltaTime());
            return moveVector;
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
