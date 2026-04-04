using RoR2;
using UnityEngine;
using RoR2.Projectile;

namespace BAMod.Tsurugi.Content
{
    public static class TsurugiAssets
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

        public static Sprite Malice;
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
            
        }

        #endregion projectiles
    }
}
