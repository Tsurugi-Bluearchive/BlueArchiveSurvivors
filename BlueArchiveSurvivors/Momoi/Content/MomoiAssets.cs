using BAMod.GlobalContent.Components;
using BAMod.Mashiro.Content;
using BAMod.Modules;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace BAMod.Momoi.Content
{
    public static class MomoiAssets
    {
        
        public static GameObject bombExplosionEffect;

        // networked hit sounds
        public static NetworkSoundEventDef swordHitSoundEvent;

        //projectiles
        public static GameObject flameGrenadePrefab;

        public static Sprite SchoolgirlSoulConsume;

        private static AssetBundle _assetBundle;
        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            CreateEffects();

            CreateProjectiles();

            GrabBuffIcons();
        }

        #region effects
        private static void CreateEffects()
        {

        }
        #endregion effects

        #region buffs
        private static void GrabBuffIcons()
        {

        }

        #endregion buffs

        #region projectiles
        private static void CreateProjectiles()
        {
            flameGrenadePrefab = Asset.CloneProjectilePrefab("CommandoGrenadeProjectile", "MomoiFlameGrenade");
            var damage = flameGrenadePrefab.GetComponent<ProjectileDamage>();
            damage.damageType.AddModdedDamageType(MomoiCustomDamageTypes.MomoiFlameGrenade);
        }

        #endregion projectiles
    }
}
