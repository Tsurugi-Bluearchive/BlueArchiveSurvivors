using BAMod.Modules;
using Rewired;
using RoR2;
using RoR2.Projectile;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Networking;

namespace BAMod.Mashiro.Content
{
    public static class MashiroAssets
    {
        // particle effects
        public static GameObject swordSwingEffect;
        public static GameObject swordHitImpactEffect;

        public static GameObject bombExplosionEffect;

        // networked hit sounds
        public static NetworkSoundEventDef swordHitSoundEvent;

        //projectiles
        public static GameObject bombProjectilePrefab;

        public static Sprite SchoolgirlSoulConsume;

        private static AssetBundle _assetBundle;

        public static bool Abort;

        public static Sprite Malice;
        public static GameObject BigProjectikle;
        public static GameObject BigGhost;
        public static GameObject SmallProjectile;
        public static GameObject SmallGhost;
        private static GameObject behemothBlast = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/BehemothExplosion");
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
            Malice = _assetBundle.LoadAsset<Sprite>("Textures/Malice");
        }

        #endregion buffs

        #region projectiles
        private static void CreateProjectiles()
        {
            BigProjectikle = Asset.CloneProjectilePrefab("CommandoGrenadeProjectile", "TankRound");
            var velocity = BigProjectikle.GetComponent<ApplyTorqueOnStart>();
            velocity.localTorque = Vector3.forward * 75;
            var collider = BigProjectikle.GetComponent<Collider>();
            collider.isTrigger = true;
            var explosive = BigProjectikle.GetComponent<ProjectileImpactExplosion>();
            explosive.blastRadius = 5;
            explosive.blastDamageCoefficient = 1f;
            explosive.destroyOnEnemy = false;
            explosive.destroyOnWorld = true;
            explosive.falloffModel = BlastAttack.FalloffModel.Linear;
            explosive.destroyOnDistance = false;
            var controller = BigProjectikle.GetComponent<ProjectileController>();
            controller.ghostPrefab = _assetBundle.LoadAsset<GameObject>("MashiroBigGhost");
            controller.procCoefficient = 3.0f;
            var pierce = BigProjectikle.AddComponent<ProjectileOverlapAttack>();
            pierce.damageCoefficient = 1f;
            pierce.overlapProcCoefficient = 3.0f;
            pierce.projectileController = BigProjectikle.GetComponent<ProjectileController>();
            pierce.canHitOwner = false;
            var simpleController = BigProjectikle.GetComponent<ProjectileSimple>();
            simpleController.desiredForwardSpeed = 5;

            SmallProjectile = Asset.CloneProjectilePrefab("CommandoGrenadeProjectile", "SnapRound");
            explosive = SmallProjectile.GetComponent<ProjectileImpactExplosion>();
            collider = SmallProjectile.GetComponent<Collider>();
            collider.isTrigger = true;
            explosive.destroyOnWorld = true;
            explosive.blastRadius = 0;
            explosive.destroyOnDistance = false;
            controller = SmallProjectile.GetComponent<ProjectileController>();
            controller.ghostPrefab = _assetBundle.LoadAsset<GameObject>("MashiroSmallGhost");
            pierce = SmallProjectile.AddComponent<ProjectileOverlapAttack>();
            pierce.damageCoefficient = 1f;
            pierce.overlapProcCoefficient = 1.0f;
            pierce.canHitOwner = false;
            pierce.projectileController = SmallProjectile.GetComponent<ProjectileController>();
            velocity = SmallProjectile.GetComponent<ApplyTorqueOnStart>();
            velocity.localTorque = Vector3.forward * 100;
            simpleController = SmallProjectile.GetComponent<ProjectileSimple>();
            simpleController.desiredForwardSpeed = 20;

        }

        #endregion projectiles
    }
}
