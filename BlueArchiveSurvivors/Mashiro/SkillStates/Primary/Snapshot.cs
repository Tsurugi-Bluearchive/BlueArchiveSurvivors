using BAMod.Mashiro.Content;
using BAMod.Mashiro;
using EntityStates.Commando.CommandoWeapon;
using R2API;
using Rewired.Demos;
using RoR2;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BAMod.Mashiro.SkillStates.BaseStates;
using EntityStates;
using RoR2.Projectile;
using BAMod.GlobalContent.Scripts;

namespace BAMod.Mashiro.SkillStates.Primary
{
    internal class Snapshot : BaseMashiroSkillState
    {
        protected override float baseDuration => 1;
        protected override float baseFireDelay => 0.5f;
        protected override float fireTime => 1;
        private bool fired = false;

        public DamageTypeCombo damageType = DamageType.Generic;
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (!fired)
                {
                    var aimRay = base.GetAimRay();
                    SimBulletManager.Fire(new SimBulletManager.SimBullet()
                    {
                        radius = 5,
                        resolution = 10,
                        damageInfo = new DamageInfo()
                        {
                            damageColorIndex = DamageColorIndex.Default,
                            damage = damageStat * MashiroStaticValues.smallGunDamageCoefficient,
                            damageType = damageType,
                            attacker = this.gameObject,
                            crit = base.RollCrit()
                        },
                        direction = aimRay.direction,
                        dropSpeed = 10,
                        maximumDistance = 300,
                        origin = aimRay.origin,
                        hitMask = BulletAttack.defaultHitMask,
                        stopperMask = LayerIndex.world.collisionMask,
                        velocity = 20,
                        owner = this.gameObject,
                        prefabIndex = MashiroAssets.MashiroSmallBullet,
                        type = GlobalContent.Components.SimBulletType.exponential,
                    });
                    fired = true;
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
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
