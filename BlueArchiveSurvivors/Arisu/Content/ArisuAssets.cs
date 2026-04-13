using BAMod.GlobalContent.Components;
using BAMod.Mashiro.Content;
using BAMod.Modules;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace BAMod.Arisu.Content
{
    public static class ArisuAssets
    {
        
        public static GameObject coreExplosionPrefab;

        // networked hit sounds
        public static NetworkSoundEventDef swordHitSoundEvent;

        //projectiles
        public static GameObject bombProjectilePrefab;

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
            coreExplosionPrefab = Asset.CloneProjectilePrefab("CommandoGrenadeProjectile", "ArisuCoreEjection");

            UnityEngine.Object.Destroy(coreExplosionPrefab.GetComponent<ProjectileImpactExplosion>());
            ProjectileImpactExplosion bombImpactExplosion = coreExplosionPrefab.AddComponent<ProjectileImpactExplosion>();

            bombImpactExplosion.blastRadius = 30f;
            bombImpactExplosion.blastDamageCoefficient = 1f;
            bombImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.lifetime = 12f;
            bombImpactExplosion.timerAfterImpact = true;
            bombImpactExplosion.lifetimeAfterImpact = 0.5f;

            ProjectileController bombController = coreExplosionPrefab.GetComponent<ProjectileController>();
            bombController.ghostPrefab = _assetBundle.LoadAsset<GameObject>("AritsuCoreEjectionGhost");
        }

        #endregion projectiles
    }
}
