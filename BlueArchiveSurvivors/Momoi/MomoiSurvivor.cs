using BAMod.Modules;
using BAMod.Modules.Characters;
using BAMod.Momoi.SkillStates.Primary;
using BAMod.Momoi.SkillStates.Secondary;
using BAMod.Momoi.SkillStates.Special;
using BAMod.Momoi.SkillStates.Utility;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using BAMod.Mashiro.Content;
using BAMod.Momoi.SkillStates.BaseStates;
using BAMod.Momoi.SkillStates.SpecialLock;
using R2API;
using BAMod.Momoi.Content;
using BAMod.Mashiro.SkillStates.BaseStates;
namespace BAMod.Momoi
{
    public class MomoiSurvivor : SurvivorBase<MomoiSurvivor>
    {
        public override string assetBundleName => "momoiassetbundle"; 
        public override string bodyName => "MomoiBody"; 
        public override string masterName => "MomoiMonsterMaster"; 
        public override string modelPrefabName => "mdlMomoi";
        public override string displayPrefabName => "MomoiDisplay";

        public const string Momoi_PREFIX = "AMI" + "_MOMOI_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => Momoi_PREFIX;
        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = Momoi_PREFIX + "NAME",
            subtitleNameToken = Momoi_PREFIX + "SUBTITLE",

            characterPortrait = LegacyResourcesAPI.Load<Texture>("RoR2/Base/Commando/texCommandoIcon"),
            bodyColor = Color.white,
            sortPosition = 100,

            crosshair = Asset.LoadCrosshair("Standard"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 120f,
            healthRegen = 1f,
            armor = 0f,
            healthGrowth = 10f,
            moveSpeed = 10f,

            jumpCount = 1,
        };

        public override UnlockableDef characterUnlockableDef => MomoiUnlockables.characterUnlockableDef;
        
        
        public override ItemDisplaysBase itemDisplays => new MomoiItemDisplays();

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[0];

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }

        public static SkillDef BurstRifleReload;

        public static SkillDef BurstRifle;

        public static SkillDef Scope;

        public static SkillDef ScopePrimaryOverride;

        public static SkillDef MomoiUlt;

        public static SkillDef MomoiStunRoll;

        public static SkillDef Lock;
        public override void Initialize()
        {
            base.Initialize();;
        }

        public override void InitializeCharacter()
        {
            //need the character unlockable before you initialize the survivordef
            MomoiUnlockables.Init();

            base.InitializeCharacter();

            MomoiConfig.Init();
            MomoiStates.Init();
            MomoiTokens.Init();

            MomoiAssets.Init(assetBundle);
            MomoiBuffs.Init();

            InitializeEntityStateMachines();
            InitializeSkills();
            AdditionalBodySetup();
            AddHooks();

            InitializeCharacterMaster();

        }

        private void AdditionalBodySetup()
        {
            bodyPrefab.AddComponent<ModelSkinController>();
            displayPrefab.AddComponent<ModelSkinController>();
            AddHitboxes();
        }

        public void AddHitboxes()
        {

        }

        public override void InitializeEntityStateMachines() 
        {

            Prefabs.ClearEntityStateMachines(bodyPrefab);

            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(MomoiCharacterMain), typeof(EntityStates.SpawnTeleporterState));

            //Weapon EntitystateMachines
            Prefabs.AddEntityStateMachine(bodyPrefab, "Gun");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Scope");

            //Movement EntityStateMachine
            Prefabs.AddEntityStateMachine(bodyPrefab, "Movement");

            //Ult EntityStateMachine
            Prefabs.AddEntityStateMachine(bodyPrefab, "Ult");

            prefabCharacterBody.vehicleIdleStateMachine = new EntityStateMachine[]
            {
                EntityStateMachine.FindByCustomName(bodyPrefab, "Gun"),
                EntityStateMachine.FindByCustomName(bodyPrefab, "Scope"),
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
            AddLockSkill();
        }
        //if this is your first look at skilldef creation, take a look at Secondary first
        private void AddPrimarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Primary);

            //the primary skill is created using a constructor for a typical primary
            //it is also a SteppedSkillDef. Custom Skilldefs are very useful for custom behaviors related to casting a skill. see ror2's different skilldefs for reference
            BurstRifle = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Flame Burst Rifle",
                skillNameToken = Momoi_PREFIX + "PRIMARY_GUN_NAME",
                skillDescriptionToken = Momoi_PREFIX + "PRIMARY_GUN_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(FlameRifle)),
                activationStateMachineName = "Gun",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 5,

                rechargeStock = 60,
                requiredStock = 1,
                stockToConsume = 1,
                baseMaxStock = 60,

                resetCooldownTimerOnUse = true,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });
            Skills.AddPrimarySkills(bodyPrefab, BurstRifle);
        }

        private void AddSecondarySkills()
        {

            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Secondary);
            Scope = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Scope",
                skillNameToken = Momoi_PREFIX + "SECONDARY_GUN_NAME",
                skillDescriptionToken = Momoi_PREFIX + "SECONDARY_GUN_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(FlameGrenade)),
                activationStateMachineName = "Scope",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 10f,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                baseMaxStock = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });

            Skills.AddSecondarySkills(bodyPrefab, Scope);
        }

        private void AddUtiitySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Utility);

            MomoiStunRoll = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "StunRoll",
                skillNameToken = Momoi_PREFIX + "UTILITY_STUN_NAME",
                skillDescriptionToken = Momoi_PREFIX + "UTILITY_STUN_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(FlameBurst)),
                activationStateMachineName = "Movement",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 5,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                baseMaxStock = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });

            Skills.AddUtilitySkills(bodyPrefab, MomoiStunRoll);
        }

        private void AddSpecialSkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Special);
            MomoiUlt = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Ultimate",
                skillNameToken = Momoi_PREFIX + "SPECIAL_ULTIMATE_NAME",
                skillDescriptionToken = Momoi_PREFIX + "SPECIAL_ULTIMATE_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(MomoiUlt)),
                activationStateMachineName = "Ult",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 120,

                rechargeStock = 1,
                requiredStock = 1,
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
            Skills.AddSpecialSkills(bodyPrefab, MomoiUlt);
        }

        private void AddLockSkill()
        {
            Lock = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Ultimate",
                skillNameToken = Momoi_PREFIX + "SPECIAL_ULTIMATE_NAME",
                skillDescriptionToken = Momoi_PREFIX + "SPECIAL_ULTIMATE_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SpecialLockDown)),
                activationStateMachineName = "Ult",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = float.MaxValue,

                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 1,
                baseMaxStock = 0,

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
            var defaultSkin = BAMod.Modules.Skins.CreateSkinDef("DEFAULT_SKIN",
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

            MomoiAI.Init(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
            MomoiHooks.Init();
        }
    }
}