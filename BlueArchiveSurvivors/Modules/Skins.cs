using R2API;
using RoR2;
using UnityEngine;

namespace BA.Modules
{
    internal static class Skins
    {
        internal static SkinDef CreateSkinDef(string skinName, Sprite skinIcon, CharacterModel.RendererInfo[] defaultRendererInfos, GameObject root, UnlockableDef unlockableDef = null)
        {
            return R2API.Skins.CreateNewSkinDef(new SkinDefInfo
            {
                Icon = skinIcon,
                Name = skinName,
                RootObject = root,
                NameToken = skinName,
                UnlockableDef = unlockableDef,
                RendererInfos = new CharacterModel.RendererInfo[defaultRendererInfos.Length],
                BaseSkins = [],
                MeshReplacements = [],
                GameObjectActivations = [],
                MinionSkinReplacements = [],
                ProjectileGhostReplacements = []
            });
        }
    }
}