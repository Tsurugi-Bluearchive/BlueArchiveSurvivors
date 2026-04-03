using BA.Modules;
using BA.Modules.Characters;
using BA.Tsurugi.Content;
using BAMod.Tsurugi.Components;
using BAMod.Tsurugi.SkillStates.Primary;
using BAMod.Tsurugi.SkillStates.Secondary;
using BAMod.Tsurugi.SkillStates.Special;
using BAMod.Tsurugi.SkillStates.Utility;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UltrakillMod.V1.SkillStates.BaseStates;
using UnityEngine;
using static RoR2.Skills.SkillFamily;
namespace BA.Tsurugi
{
    public class TsurugiSurvivor : SurvivorBase<TsurugiSurvivor>
    {
        public override string assetBundleName => "v1assetbundle"; 
        public override string bodyName => "V1Body"; 
        public override string masterName => "V1MonsterMaster"; 
        public override string modelPrefabName => "mdlV1";
        public override string displayPrefabName => "V1Display";

        public const string V1_PREFIX = "AMI" + "_V1_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => V1_PREFIX;
        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = V1_PREFIX + "NAME",
            subtitleNameToken = V1_PREFIX + "SUBTITLE",

            characterPortrait = LegacyResourcesAPI.Load<Texture>("RoR2/Base/Commando/texCommandoIcon"),
            bodyColor = Color.white,
            sortPosition = 100,

            crosshair = Asset.LoadCrosshair("Standard"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 90f,
            healthRegen = 3f,
            armor = 0f,
            healthGrowth = 10f,
            moveSpeed = 10f,

            jumpCount = 2,
        };

        public override UnlockableDef characterUnlockableDef => TsurugiUnlockables.characterUnlockableDef;
        
        public override ItemDisplaysBase itemDisplays => new TsurugiItemDisplays();

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[0];

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }

        public static SkillDef BloodReload;

        public static SkillDef Blood;

        public static SkillDef Gunpowder;

        public static SkillDef GunpowderReload;

        public static SkillDef TsurugiUlt;

        public static SkillDef TsurugiStunRoll;
        public override void Initialize()
        {
            base.Initialize();;
        }

        public override void InitializeCharacter()
        {
            //need the character unlockable before you initialize the survivordef
            TsurugiUnlockables.Init();

            base.InitializeCharacter();

            TsurugiConfig.Init();
            TsurugiStates.Init();
            TsurugiTokens.Init();

            TsurugiAssets.Init(assetBundle);

            InitializeEntityStateMachines();
            InitializeSkills();
            AdditionalBodySetup();
            AddHooks();

            InitializeCharacterMaster();

        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bodyPrefab.AddComponent<CrazyWomanPassiveDictator>();
        }

        public void AddHitboxes()
        {

        }

        public override void InitializeEntityStateMachines() 
        {

            Prefabs.ClearEntityStateMachines(bodyPrefab);

            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(TsurugiCharacterMain), typeof(EntityStates.SpawnTeleporterState));

            //Weapon EntitystateMachines
            Prefabs.AddEntityStateMachine(bodyPrefab, "RightShotgun");
            Prefabs.AddEntityStateMachine(bodyPrefab, "LeftShotgun");

            //Movement EntityStateMachine
            Prefabs.AddEntityStateMachine(bodyPrefab, "Movement");

            //Ult EntityStateMachine
            Prefabs.AddEntityStateMachine(bodyPrefab, "Ult");

            prefabCharacterBody.vehicleIdleStateMachine = new EntityStateMachine[]
            {
                EntityStateMachine.FindByCustomName(bodyPrefab, "RightShotgun"),
                EntityStateMachine.FindByCustomName(bodyPrefab, "LeftShotgun"),
                EntityStateMachine.FindByCustomName(bodyPrefab, "Movement"),
                EntityStateMachine.FindByCustomName(bodyPrefab, "Ult")
            };
        }

        #region skills
        public override void InitializeSkills()
        {
            //remove the genericskills from the commando body we cloned
            Skills.ClearGenericSkills(bodyPrefab);
            //add our own
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtiitySkills();
            AddSpecialSkills();
        }
        //if this is your first look at skilldef creation, take a look at Secondary first
        private void AddPrimarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Primary);

            //the primary skill is created using a constructor for a typical primary
            //it is also a SteppedSkillDef. Custom Skilldefs are very useful for custom behaviors related to casting a skill. see ror2's different skilldefs for reference
            Blood = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Blood",
                skillNameToken = V1_PREFIX + "PRIMARY_GUN_NAME",
                skillDescriptionToken = V1_PREFIX + "PRIMARY_GUN_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(Blood)),
                activationStateMachineName = "LeftShotgun",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = float.MaxValue,

                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 1,
                baseMaxStock = 3,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });

            BloodReload = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Blood_Reload",
                skillNameToken = V1_PREFIX + "PRIMARY_RELOAD_NAME",
                skillDescriptionToken = V1_PREFIX + "PRIMARY_RELOAD_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(BloodReload)),
                activationStateMachineName = "LeftShotgun",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = float.MaxValue,

                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0,
                baseMaxStock = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddPrimarySkills(bodyPrefab, Blood);
        }

        private void AddSecondarySkills()
        {
            Gunpowder = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Gunpowder",
                skillNameToken = V1_PREFIX + "SECONDARY_GUN_NAME",
                skillDescriptionToken = V1_PREFIX + "SECONDARY_GUN_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(Gunpowder)),
                activationStateMachineName = "RightShotgun",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = float.MaxValue,

                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 1,
                baseMaxStock = 3,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });

            GunpowderReload = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Gunpowder_Reload",
                skillNameToken = V1_PREFIX + "SECONDARY_RELOAD_NAME",
                skillDescriptionToken = V1_PREFIX + "SECONDARY_RELOAD_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(GunpowderReload)),
                activationStateMachineName = "LeftShotgun",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = float.MaxValue,

                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0,
                baseMaxStock = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });
            Skills.AddSecondarySkills(bodyPrefab, Gunpowder);
        }

        private void AddUtiitySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Utility);

            TsurugiStunRoll = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "StunRoll",
                skillNameToken = V1_PREFIX + "UTILITY_STUN_NAME",
                skillDescriptionToken = V1_PREFIX + "PRIMARY_GUN_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(StunRoll)),
                activationStateMachineName = "Movement",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 3,

                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 1,
                baseMaxStock = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });

            Skills.AddUtilitySkills(bodyPrefab, TsurugiStunRoll);
        }

        private void AddSpecialSkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Special);
            TsurugiUlt = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Ultimate",
                skillNameToken = V1_PREFIX + "SPECIAL_ULTIMATE_NAME",
                skillDescriptionToken = V1_PREFIX + "PRIMARY_GUN_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(TsurugiUlt)),
                activationStateMachineName = "Ult",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 20,

                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 1,
                baseMaxStock = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });
            Skills.AddSpecialSkills(bodyPrefab, TsurugiUlt);
        }

        #endregion skills
        
        #region skins
        public override void InitializeSkins()
        {
            var skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            var defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            var skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            var defaultSkin = BA.Modules.Skins.CreateSkinDef("DEFAULT_SKIN",
                assetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
                //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
                //uncomment this when you have another skin
            //defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySword",
            //    "meshHenryGun",
            //    "meshHenry");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin
            
            ////creating a new skindef as we did before
            //SkinDef masterySkin = Modules.Skins.CreateSkinDef(MAMI_PREFIX + "MASTERY_SKIN_NAME",
            //    assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
            //    defaultRendererinfos,
            //    prefabCharacterModel.gameObject,
            //    HenryUnlockables.masterySkinUnlockableDef);

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            //masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySwordAlt",
            //    null,//no gun mesh replacement. use same gun mesh
            //    "meshHenryAlt");

            ////masterySkin has a new set of RendererInfos (based on default rendererinfos)
            ////you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            //masterySkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[2].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");

            ////here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            //masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            //{
            //    new SkinDef.GameObjectActivation
            //    {
            //        gameObject = childLocator.FindChildGameObject("GunModel"),
            //        shouldActivate = false,
            //    }
            //};
            ////simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            //skins.Add(masterySkin);
            
            #endregion

            skinController.skins = skins.ToArray();
        }
        #endregion skins

        public override void InitializeCharacterMaster()
        {

            TsurugiAI.Init(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
            TsurugiHooks.Init();            
        }
    }
}