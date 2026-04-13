using BAMod.GlobalContent.Components;
using BAMod.GlobalContent.Scripts;
using BAMod.Modules;
using Rewired;
using RoR2;
using RoR2.Projectile;
using RoR2BepInExPack.GameAssetPaths;
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

        public static int MashiroBigBullet;
        public static int MashiroSmallBullet;

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
            BigProjectikle = _assetBundle.LoadAsset<GameObject>("MashiroBigGhost");
            SimBulletManager.RegisterSimBulletObject(out MashiroBigBullet, BigProjectikle, "Mashiro Big Bullet");

            SmallProjectile = _assetBundle.LoadAsset<GameObject>("MashiroSmallGhost");
            SimBulletManager.RegisterSimBulletObject(out MashiroSmallBullet, SmallProjectile, "Mashiro Small Bullet");
        }

        #endregion projectiles
    }
}
