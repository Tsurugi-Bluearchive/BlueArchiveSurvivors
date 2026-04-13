using BAMod.Modules;
using BAMod.Modules.Characters;
using BAMod.Arisu.SkillStates.Primary;
using BAMod.Arisu.SkillStates.Secondary;
using BAMod.Arisu.SkillStates.Special;
using BAMod.Arisu.SkillStates.Utility;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using BAMod.Mashiro.Content;
using BAMod.Arisu.SkillStates.BaseStates;
using BAMod.Arisu.SkillStates.SpecialLock;
using R2API;
using BAMod.Arisu.Content;
using BAMod.Mashiro.SkillStates.BaseStates;
namespace BAMod.Arisu
{
    public class ArisuSurvivor : SurvivorBase<ArisuSurvivor>
    {
        public override string assetBundleName => "arisuassetbundle"; 
        public override string bodyName => "ArisuBody"; 
        public override string masterName => "ArisuMonsterMaster"; 
        public override string modelPrefabName => "mdlArisu";
        public override string displayPrefabName => "ArisuDisplay";

        public const string Arisu_PREFIX = "AMI" + "_ARISU_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => Arisu_PREFIX;
        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = Arisu_PREFIX + "NAME",
            subtitleNameToken = Arisu_PREFIX + "SUBTITLE",

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

        public override UnlockableDef characterUnlockableDef => ArisuUnlockables.characterUnlockableDef;
        
        
        public override ItemDisplaysBase itemDisplays => new ArisuItemDisplays();

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[0];

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }

        public static SkillDef Beam;
        public static SkillDef BeamOverheat;
        public static SkillDef UltBeam;
        public static SkillDef UltBeamOverheat;
        public static SkillDef CoreEject;

        public static SkillDef ArisuUlt;

        public static SkillDef EmergencyCooling;

        public static SkillDef Lock;
        public override void Initialize()
        {
            base.Initialize();;
        }

        public override void InitializeCharacter()
        {
            //need the character unlockable before you initialize the survivordef
            ArisuUnlockables.Init();

            base.InitializeCharacter();

            ArisuConfig.Init();
            ArisuStates.Init();
            ArisuTokens.Init();

            ArisuAssets.Init(assetBundle);
            ArisuBuffs.Init();

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

            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(ArisuCharacterMain), typeof(EntityStates.SpawnTeleporterState));

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
            AddUltPrimary();

        }
        //if this is your first look at skilldef creation, take a look at Secondary first
        private void AddPrimarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Primary);

            //the primary skill is created using a constructor for a typical primary
            //it is also a SteppedSkillDef. Custom Skilldefs are very useful for custom behaviors related to casting a skill. see ror2's different skilldefs for reference
            Beam = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Flame Burst Rifle",
                skillNameToken = Arisu_PREFIX + "PRIMARY_GUN_NAME",
                skillDescriptionToken = Arisu_PREFIX + "PRIMARY_GUN_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(BeamAttack)),
                activationStateMachineName = "Gun",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 10,

                rechargeStock = 100,
                requiredStock = 1,
                stockToConsume = 1,
                baseMaxStock = 100,

                resetCooldownTimerOnUse = true,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = false,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });

            BeamOverheat = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Flame Burst Rifle Reload",
                skillNameToken = Arisu_PREFIX + "PRIMARY_RELOAD_NAME",
                skillDescriptionToken = Arisu_PREFIX + "PRIMARY_RELOAD_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(BeamAttackOverheat)),
                activationStateMachineName = "Gun",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = float.MaxValue,

                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0,
                baseMaxStock = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddPrimarySkills(bodyPrefab, Beam);
        }

        private void AddSecondarySkills()
        {

            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Secondary);
            CoreEject = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Scope",
                skillNameToken = Arisu_PREFIX + "SECONDARY_GUN_NAME",
                skillDescriptionToken = Arisu_PREFIX + "SECONDARY_GUN_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(CoreEject)),
                activationStateMachineName = "Scope",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 25f,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                baseMaxStock = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });

            Skills.AddSecondarySkills(bodyPrefab, CoreEject);
        }

        private void AddUtiitySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Utility);

            EmergencyCooling = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "StunRoll",
                skillNameToken = Arisu_PREFIX + "UTILITY_STUN_NAME",
                skillDescriptionToken = Arisu_PREFIX + "UTILITY_STUN_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(EmergencyCooling)),
                activationStateMachineName = "Movement",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 20f,

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

            Skills.AddUtilitySkills(bodyPrefab, EmergencyCooling);
        }

        private void AddSpecialSkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Special);
            ArisuUlt = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Ultimate",
                skillNameToken = Arisu_PREFIX + "SPECIAL_ULTIMATE_NAME",
                skillDescriptionToken = Arisu_PREFIX + "SPECIAL_ULTIMATE_DESCRIPTION",
                keywordTokens = ["KEYWORD_AGILE"],
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(ArisuUlt)),
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

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });
            Skills.AddSpecialSkills(bodyPrefab, ArisuUlt);
        }

        private void AddLockSkill()
        {
            Lock = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Ultimate",
                skillNameToken = Arisu_PREFIX + "SPECIAL_ULTIMATE_NAME",
                skillDescriptionToken = Arisu_PREFIX + "SPECIAL_ULTIMATE_DESCRIPTION",
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

        private void AddUltPrimary()
        {
            GenericSkill passiveGenericSkill = Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, "PassiveSkill");
            UltBeam = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "UltimatePrimary",
                skillNameToken = Arisu_PREFIX + "SPECIAL_ULTIMATE_PRIMARY_NAME",
                skillDescriptionToken = Arisu_PREFIX + "SPECIAL_ULTIMATE_PRIMARY_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE" },
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),


                activationState = new EntityStates.SerializableEntityStateType(typeof(ArisuUltBeamAttack)),
                activationStateMachineName = "Gun",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = float.MaxValue,

                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 1,
                baseMaxStock = 100,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            UltBeamOverheat = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "UltimatePrimary",
                skillNameToken = Arisu_PREFIX + "ULT_PRIMARY_NAME",
                skillDescriptionToken = Arisu_PREFIX + "ULT_PRIMARY_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE" },
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),


                activationState = new EntityStates.SerializableEntityStateType(typeof(ArisuUltBeamAttackOverheat)),
                activationStateMachineName = "Gun",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = float.MaxValue,

                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0,
                baseMaxStock = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });


            Skills.AddSkillsToFamily(passiveGenericSkill.skillFamily, UltBeam);
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

            ArisuAI.Init(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
            ArisuHooks.Init();
        }
    }
}