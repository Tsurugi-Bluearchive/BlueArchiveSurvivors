using BAMod.GlobalContent.Components;
using BAMod.Arisu.Content;
using BAMod.Arisu.SkillStates.BaseStates;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using R2API;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using RoR2.Projectile;

namespace BAMod.Arisu.SkillStates.Secondary
{
    internal class CoreEject : BaseArisuSkillState
    {
        protected override float baseDuration => 15;
        protected override float baseFireDelay => 0.5f;
        protected override float fireTime => 1;
        private bool fired = false;
        public GameObject hitEffectPrefab = FireBarrage.hitEffectPrefab;
        public GameObject tracerEffectPrefab = FireBarrage.tracerEffectPrefab;
        public DamageTypeCombo damageType = DamageType.Generic;
        private bool ejected;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isAuthority) return;
            if (skillLocator.primary.stock <= 0)
            {
                activatorSkillSlot.AddOneStock();
                outer.SetNextStateToMain();
                return;
            }
            if (!ejected)
            {
                var aimRay = GetAimRay();
                var projectile = new FireProjectileInfo()
                {
                    damage = skillLocator.primary.stock * ArisuStaticValues.coreEjectMult * damageStat,
                    damageColorIndex = DamageColorIndex.Electrocution,
                    crit = RollCrit(),
                    projectilePrefab = ArisuAssets.coreExplosionPrefab,
                    owner = this.gameObject,
                    position = this.gameObject.transform.position,
                    force = 80f,
                    rotation = Quaternion.LookRotation(aimRay.direction),
                    maxDistance = 300f,
                };
                ProjectileManager.instance.FireProjectile(projectile);
                skillLocator.primary.stock = 0;
                skillLocator.primary.cooldownOverride = 20;
                ejected = true;
                if (ArisuMain.ultimateGun)
                {
                    skillLocator.primary.UnsetSkillOverride(this.gameObject, ArisuSurvivor.UltBeam, GenericSkill.SkillOverridePriority.Default);
                    ArisuMain.ultimateGun = false;
                }
            }
            if (ejected && fixedAge > duration)
            {
                skillLocator.primary.cooldownOverride = 0;
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
