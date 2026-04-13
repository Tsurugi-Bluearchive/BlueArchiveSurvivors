using BAMod.GlobalContent.Scripts;
using BAMod.Mashiro.Content;
using BAMod.Mashiro.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace BAMod.Mashiro.SkillStates.Secondary
{
    internal class BigRound : BaseMashiroSkillState
    {
        protected override float baseDuration => 2f;
        protected override float baseFireDelay => 0.2f;
        protected override float fireTime => 1f;

        private bool fired = false;
        private Vector3 recoilDirection;
        private float recoilStrength = 35f;
        DamageTypeCombo damageType = DamageTypeCombo.GenericSecondary;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!isAuthority)
                return;

            var aimRay = GetAimRay();

            // Fire the bullet
            if (!fired && fixedAge >= baseFireDelay)
            {
                SimBulletManager.Fire(new SimBulletManager.SimBullet()
                {
                    radius = 1f,
                    resolution = 10,
                    damageInfo = new DamageInfo()
                    {
                        damageColorIndex = DamageColorIndex.Default,
                        damage = damageStat * MashiroStaticValues.bigGunDamageCefficeient,
                        damageType = damageType,
                        attacker = gameObject,
                        crit = RollCrit()
                    },
                    direction = aimRay.direction,
                    dropSpeed = 5,
                    maximumDistance = 300,
                    origin = aimRay.origin + aimRay.direction * 2f,
                    hitMask = BulletAttack.defaultHitMask,
                    stopperMask = LayerIndex.world.mask,
                    velocity = 100,
                    owner = gameObject,
                    prefabIndex = MashiroAssets.MashiroBigBullet,
                    type = SimBulletType.exponential,
                    falloffModel = BlastAttack.FalloffModel.None,
                    explodeOnExpire = true
                }, MashiroMain.NetworkBehavior,
                MashiroAssets.MashiroBigBullet);

                fired = true;
                characterBody.characterMotor.Motor.ForceUnground();
                characterBody.characterMotor.ApplyForceImpulse(new PhysForceInfo()
                {
                    force = -aimRay.direction * recoilStrength,
                    ignoreGroundStick = true,
                    respectKnockupImmune = true,
                    resetVelocity = false
                });
            }

            if (fixedAge > duration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (!fired)
            {
                activatorSkillSlot?.AddOneStock();
            }
        }
    }
}