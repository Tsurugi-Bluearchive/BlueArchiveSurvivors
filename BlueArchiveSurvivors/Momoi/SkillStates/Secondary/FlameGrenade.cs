using BAMod.Arisu.Content;
using BAMod.GlobalContent.Components;
using BAMod.Momoi.Content;
using BAMod.Momoi.SkillStates.BaseStates;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BAMod.Momoi.SkillStates.Secondary
{
    internal class FlameGrenade : BaseMomoiSkillState
    {
        protected override float baseDuration => 4;
        protected override float baseFireDelay => 0.5f;
        protected override float fireTime => 1;
        private bool fired = false;
        public GameObject hitEffectPrefab = FireBarrage.hitEffectPrefab;
        public GameObject tracerEffectPrefab = FireBarrage.tracerEffectPrefab;
        public DamageTypeCombo damageType = DamageType.Generic;
        private bool flamed;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isAuthority) return;
            if (!flamed)
            {
                var aimRay = GetAimRay();
                var projectile = new FireProjectileInfo()
                {
                    damage = skillLocator.primary.stock * ArisuStaticValues.coreEjectMult * damageStat,
                    damageColorIndex = DamageColorIndex.Electrocution,
                    crit = RollCrit(),
                    projectilePrefab = MomoiAssets.flameGrenadePrefab,
                    owner = this.gameObject,
                    position = this.gameObject.transform.position,
                    force = 80f,
                    rotation = Quaternion.LookRotation(aimRay.direction),
                    maxDistance = 300f,
                };
                ProjectileManager.instance.FireProjectile(projectile);
                flamed = true;
            }
            if (flamed && fixedAge > duration)
            {
                outer.SetNextStateToMain();
                return;
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
