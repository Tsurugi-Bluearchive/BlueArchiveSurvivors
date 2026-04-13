using RoR2;
using UnityEngine;
using RoR2.Projectile;
using BAMod.GlobalContent.Components;
using BAMod.Mashiro.Content;
using R2API;
using UnityEngine.Networking.Types;

namespace BAMod.Saori.Content
{
    public static class SaoriAssets
    {
        // particle effects
        public static GameObject swordSwingEffect;
        public static GameObject swordHitImpactEffect;
        public static GameObject mainMarkBuffEffect;
        public static GameObject secondaryMarkBuffEffect;
        public static GameObject markTargetEffect;
        public static GameObject markSecondaryTargetEffect;

        public static GameObject bombExplosionEffect;

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
            mainMarkBuffEffect = PrefabAPI.InstantiateClone(_assetBundle.LoadAsset<GameObject>("UIMainMark"), "Main Mark Buff");
            var controller = mainMarkBuffEffect.AddComponent<DisplayAboveModelTransform>();
            controller.parentBuff = SaoriBuffs.SaoriPrimaryMarkBuff;
            controller.enabled = false;

            secondaryMarkBuffEffect = PrefabAPI.InstantiateClone(_assetBundle.LoadAsset<GameObject>("UISecondaryMark"), "Secondary Mark Buff");
            controller = secondaryMarkBuffEffect.AddComponent<DisplayAboveModelTransform>();
            controller.parentBuff = SaoriBuffs.SaoriMarkBuff;

            markTargetEffect = PrefabAPI.InstantiateClone(_assetBundle.LoadAsset<GameObject>("UIMainMarkSkill"), "Main Mark Skill");
            controller = markTargetEffect.AddComponent<DisplayAboveModelTransform>();

            markSecondaryTargetEffect = PrefabAPI.InstantiateClone(_assetBundle.LoadAsset<GameObject>("UISecondaryMarkSkill"), "Secondary Mark Skill");
            controller = markSecondaryTargetEffect.AddComponent<DisplayAboveModelTransform>();

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
            
        }

        #endregion projectiles
    }
}
