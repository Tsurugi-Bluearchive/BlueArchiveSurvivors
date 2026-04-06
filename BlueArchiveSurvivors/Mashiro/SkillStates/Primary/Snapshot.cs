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
                    var snapshotround = new FireProjectileInfo()
                    {
                        crit = base.RollCrit(),
                        damage = MashiroStaticValues.smallGunDamageCoefficient * damageStat,
                        fuseOverride = float.MaxValue,
                        useSpeedOverride = true,
                        owner = this.characterBody.gameObject,
                        position = aimRay.origin + aimRay.direction * 2,
                        rotation = Quaternion.LookRotation(aimRay.direction),
                        projectilePrefab = MashiroAssets.SmallProjectile,
                        force = 200,
                        speedOverride = 400
                    };
                    ProjectileManager.instance.FireProjectile(snapshotround);
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
