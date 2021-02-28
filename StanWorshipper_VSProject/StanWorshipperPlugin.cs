using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using RoR2.Projectile;
using RoR2.CharacterAI;
using EntityStates;
using BepInEx;
using R2API;
using R2API.Utils;
using R2API.Networking;
using HG;
using KinematicCharacterController;

namespace StanWorshipper
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin("com.Vodhr.StanWorshipperSurvivor", "Stan Worshipper Survivor", "0.0.3")]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "SurvivorAPI",
        "LoadoutAPI",
        "BuffAPI",
        "LanguageAPI",
        "SoundAPI",
        "EffectAPI",
        "UnlockablesAPI",
        "ResourcesAPI",
        "NetworkingAPI"
    })]

    public class StanWorshipperPlugin : BaseUnityPlugin
    {
        public static GameObject stanWorshipperBody;
        public static GameObject stanWorshipperDislay;
        public static GameObject stanWorshipperDoppelganger;

        public static GameObject poopslingerProjectile;
        public static GameObject poopslingerExplosionEffect;

        public static GameObject sacrificeBaseEffect;
        public static GameObject sacrificeStanEffect;

        public static GameObject weakStanFragmentMaster;
        public static GameObject weakStanFragmentBody;
        public static GameObject wispBodyClone;
        public static GameObject weakStanBurnEffect;

        public static GameObject strongStanFragmentMaster;
        public static GameObject strongStanFragmentBody;

        public static BuffIndex sacrificeBuff;

        public static BepInEx.Logging.ManualLogSource logger;

        public void Awake()
        {
            Assets.InitializeAssets();
            CreateHooks();
            CreateCharacter();
            InitializeCharacter();
            InitializeSounds();
            InitializeEffects();
            InitializeSkills();
            InitializeBuffs();
            InitializeProjectiles();
            InitializeSummons();
            InitializeINetMessages();
            CreateDoppelganger();

            logger = this.Logger;
        }

        private void CreateHooks()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        internal static void CreateCharacter()
        {
            LanguageAPI.Add("STANWORSHIPPER_DESCRIPTION", "An apostle of Stan." + Environment.NewLine);
            LanguageAPI.Add("STANWORSHIPPER_NAME", "Stan Worshipper" + Environment.NewLine);
            LanguageAPI.Add("STANWORSHIPPER_SUBTITLE", "Crazed, Fat, and Horny" + Environment.NewLine);
            LanguageAPI.Add("STANWORSHIPPER_OUTRO_FLAVOR", "..and so he left, horny as fuck.");

            stanWorshipperBody = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/characterbodies/BanditBody"), "StanWorshipperBody", true);
            stanWorshipperBody.GetComponent<NetworkIdentity>().localPlayerAuthority = true;

            CharacterBody bodyComponent = stanWorshipperBody.GetComponent<CharacterBody>();
            bodyComponent.bodyIndex = -1;
            bodyComponent.baseNameToken = "STANWORSHIPPER_NAME";
            bodyComponent.subtitleNameToken = "STANWORSHIPPER_SUBTITLE";
            bodyComponent.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes;
            bodyComponent.rootMotionInMainState = false;
            bodyComponent.mainRootSpeed = 0;
            bodyComponent.baseMaxHealth = 84;
            bodyComponent.levelMaxHealth = 24;
            bodyComponent.baseRegen = 0.5f;
            bodyComponent.levelRegen = 0.25f;
            bodyComponent.baseMaxShield = 0;
            bodyComponent.levelMaxShield = 0;
            bodyComponent.baseMoveSpeed = 7;
            bodyComponent.levelMoveSpeed = 0;
            bodyComponent.baseAcceleration = 80;
            bodyComponent.baseJumpPower = 15;
            bodyComponent.levelJumpPower = 0;
            bodyComponent.baseDamage = 13;
            bodyComponent.levelDamage = 3.5f;
            bodyComponent.baseAttackSpeed = 1;
            bodyComponent.levelAttackSpeed = 0;
            bodyComponent.baseCrit = 1;
            bodyComponent.levelCrit = 0;
            bodyComponent.baseArmor = 5;
            bodyComponent.levelArmor = 0;
            bodyComponent.baseJumpCount = 1;
            bodyComponent.sprintingSpeedMultiplier = 1.45f;
            bodyComponent.wasLucky = false;
            bodyComponent.hideCrosshair = false;
            bodyComponent.hullClassification = HullClassification.Human;
            bodyComponent.portraitIcon = Resources.Load<Texture>("textures/bodyicons/ScavBody");
            bodyComponent.isChampion = false;
            bodyComponent.currentVehicle = null;
            bodyComponent.skinIndex = 0U;

            NetworkStateMachine networkStateMachine = stanWorshipperBody.GetComponent<NetworkStateMachine>();
            EntityStateMachine customEntityStateMachine = stanWorshipperBody.AddComponent<EntityStateMachine>();
            customEntityStateMachine.customName = "Sacrifice";
            customEntityStateMachine.mainStateType = new SerializableEntityStateType(typeof(Idle));
            customEntityStateMachine.initialStateType = new SerializableEntityStateType(typeof(Idle));
            ArrayUtils.ArrayAppend(ref networkStateMachine.stateMachines, customEntityStateMachine);

            //Adds a default placeholder skin to prevent weird errors (another disgusting waste of my time :D).
            GameObject model = stanWorshipperBody.GetComponent<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();
            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            CharacterModel.RendererInfo[] renderInfos = characterModel.baseRendererInfos;
            LoadoutAPI.SkinDefInfo skinDefInfo = new LoadoutAPI.SkinDefInfo
            {
                BaseSkins = Array.Empty<SkinDef>(),
                GameObjectActivations = new SkinDef.GameObjectActivation[0],
                Icon = Assets.emptyIcon,
                MeshReplacements = new SkinDef.MeshReplacement[0],
                MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0],
                Name = "STANWORSHIPPER_NAME",
                NameToken = "STANWORSHIPPER_NAME",
                ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0],
                RendererInfos = renderInfos,
                RootObject = model,
                UnlockableName = "",
            };
            SkinDef skin = LoadoutAPI.CreateNewSkinDef(skinDefInfo);
            skinController.skins = new SkinDef[] { skin };
        }

        private void InitializeCharacter() 
        {
            stanWorshipperDislay = PrefabAPI.InstantiateClone(stanWorshipperBody.GetComponent<ModelLocator>().modelBaseTransform.gameObject, "StanWorshipperDisplay", true);
            stanWorshipperDislay.AddComponent<NetworkIdentity>();

            var StanWorshipper = new SurvivorDef
            {
                bodyPrefab = stanWorshipperBody,
                name = "STANWORSHIPPER_NAME",
                displayNameToken = "STANWORSHIPPER_NAME",
                descriptionToken = "STANWORSHIPPER_DESCRIPTION",
                outroFlavorToken = "STANWORSHIPPER_OUTRO_FLAVOR",
                displayPrefab = stanWorshipperDislay,
                primaryColor = new Color(0.6f, 0.3f, 0f),
                unlockableName = "",
            };
            SurvivorAPI.AddSurvivor(StanWorshipper);

            BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(stanWorshipperBody);
            };
        }

        private void InitializeSounds()
        {

        }

        private void InitializeEffects()
        {
            poopslingerExplosionEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/effects/impacteffects/BeetleSpitExplosion"), "PoopslingerExplosionEffect", true);
            if (!poopslingerExplosionEffect.GetComponent<NetworkIdentity>()) poopslingerExplosionEffect.AddComponent<NetworkIdentity>();
            poopslingerExplosionEffect.GetComponent<EffectComponent>().soundName = Assets.PoopslingerHitSound;
            EffectAPI.AddEffect(poopslingerExplosionEffect);

            sacrificeBaseEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/effects/ArtifactShellExplosion"), "SacrificeBaseEffect", true);
            if (!sacrificeBaseEffect.GetComponent<NetworkIdentity>()) sacrificeBaseEffect.AddComponent<NetworkIdentity>();
            sacrificeBaseEffect.GetComponentInChildren<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().blendDistance = 100f;
            sacrificeBaseEffect.transform.Find("AreaIndicator").gameObject.SetActive(false);
            sacrificeBaseEffect.transform.Find("Vacuum Stars, Distortion").gameObject.SetActive(false);
            sacrificeBaseEffect.transform.Find("Point light").gameObject.SetActive(false);
            EffectAPI.AddEffect(sacrificeBaseEffect);

            sacrificeStanEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/effects/ArtifactShellExplosion"), "SacrificeStanEffect", true);
            if (!sacrificeStanEffect.GetComponent<NetworkIdentity>()) sacrificeStanEffect.AddComponent<NetworkIdentity>();
            sacrificeBaseEffect.GetComponentInChildren<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().blendDistance = 100f;
            sacrificeStanEffect.transform.Find("Point light").gameObject.SetActive(false);
            EffectAPI.AddEffect(sacrificeStanEffect);

            weakStanBurnEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/effects/impacteffects/Explosionsolarflare"), "WeakStanBurnEffect", true);
            if (!weakStanBurnEffect.GetComponent<NetworkIdentity>()) weakStanBurnEffect.AddComponent<NetworkIdentity>();
            weakStanBurnEffect.GetComponent<EffectComponent>().soundName = "";
            EffectAPI.AddEffect(weakStanBurnEffect);
        }

        private void InitializeSkills()
        {

            LanguageAPI.Add("STANWORSHIPPER_SLOT1_PRIMARY_NAME", "Poopslinger");
            LanguageAPI.Add("STANWORSHIPPER_SLOT1_PRIMARY_DESCRIPTION", "Launches a smelly fart glob which deals <style=cIsDamage>650% damage</style>.");
            LanguageAPI.Add("STANWORSHIPPER_SLOT2_SECONDARY_NAME", "Summon Minor Stan Fragments");
            LanguageAPI.Add("STANWORSHIPPER_SLOT2_SECONDARY_DESCRIPTION", "Summons <style=cIsUtility>3 weak Stan fragments</style>. Each fragment burns enemies in a 5m radius around " +
                "itself for <style=cIsDamage>140% damage</style>. <style=cIsUtility>Inherits most of your current stats</style>. ");
            LanguageAPI.Add("STANWORSHIPPER_SLOT3_UTILITY_NAME", "Sacrifice");
            LanguageAPI.Add("STANWORSHIPPER_SLOT3_UTILITY_DESCRIPTION", "Deals <style=cIsDamage>175% damage</style> every second for 7 seconds in a 30m radius around you." +
                " Also heals you for <style=cIsHealth>8% of your maximum health</style>. This ability also manifests on Stan Fragments, with Minor Stan Fragments recieving half of the healing but dealing no damage " +
                "and Major Stan Fragments recieving no healing but dealing double damage. <style=cIsUtility>Scales with attack speed</style>.");
            LanguageAPI.Add("STANWORSHIPPER_SLOT4_ULTIMATE_NAME", "Summon Major Stan Fragment");
            LanguageAPI.Add("STANWORSHIPPER_SLOT4_ULTIMATE_DESCRIPTION", "Summons <style=cIsUtility>a powerful Stan fragment</style>. Uses a more powerful version of Poopslinger, dealing <style=cIsDamage>5x190% damage</style>. " +
                "<style=cIsUtility>Inherits most of your current stats</style>.");

            //Primary
            var poopSlinger = ScriptableObject.CreateInstance<SkillDef>();
            poopSlinger.activationState = new SerializableEntityStateType(typeof(States.Poopslinger));
            poopSlinger.activationStateMachineName = "Weapon";
            poopSlinger.baseMaxStock = 1;
            poopSlinger.baseRechargeInterval = 0f;
            poopSlinger.beginSkillCooldownOnSkillEnd = true;
            poopSlinger.canceledFromSprinting = false;
            poopSlinger.fullRestockOnAssign = true;
            poopSlinger.interruptPriority = InterruptPriority.Any;
            poopSlinger.isBullets = false;
            poopSlinger.isCombatSkill = true;
            poopSlinger.mustKeyPress = false;
            poopSlinger.noSprint = true;
            poopSlinger.rechargeStock = 1;
            poopSlinger.requiredStock = 1;
            poopSlinger.shootDelay = 0f;
            poopSlinger.stockToConsume = 1;
            poopSlinger.icon = Assets.icon1;
            poopSlinger.skillDescriptionToken = "STANWORSHIPPER_SLOT1_PRIMARY_DESCRIPTION";
            poopSlinger.skillName = "STANWORSHIPPER_SLOT1_PRIMARY_NAME";
            poopSlinger.skillNameToken = "STANWORSHIPPER_SLOT1_PRIMARY_NAME";

            //Secondary
            var summonMinor = ScriptableObject.CreateInstance<SkillDef>();
            summonMinor.activationState = new SerializableEntityStateType(typeof(States.SummonMinor));
            summonMinor.activationStateMachineName = "Weapon";
            summonMinor.baseMaxStock = 1;
            summonMinor.baseRechargeInterval = 25f;
            summonMinor.beginSkillCooldownOnSkillEnd = true;
            summonMinor.canceledFromSprinting = false;
            summonMinor.fullRestockOnAssign = true;
            summonMinor.interruptPriority = InterruptPriority.Skill;
            summonMinor.isBullets = false;
            summonMinor.isCombatSkill = true;
            summonMinor.mustKeyPress = false;
            summonMinor.noSprint = false;
            summonMinor.rechargeStock = 1;
            summonMinor.requiredStock = 1;
            summonMinor.shootDelay = 0;
            summonMinor.stockToConsume = 1;
            summonMinor.icon = Assets.icon2;
            summonMinor.skillDescriptionToken = "STANWORSHIPPER_SLOT2_SECONDARY_DESCRIPTION";
            summonMinor.skillName = "STANWORSHIPPER_SLOT2_SECONDARY_NAME";
            summonMinor.skillNameToken = "STANWORSHIPPER_SLOT2_SECONDARY_NAME";

            //Utility
            var sacrifice = ScriptableObject.CreateInstance<SkillDef>();
            sacrifice.activationState = new SerializableEntityStateType(typeof(States.Sacrifice));
            sacrifice.activationStateMachineName = "Sacrifice";
            sacrifice.baseMaxStock = 1;
            sacrifice.baseRechargeInterval = 16f;
            sacrifice.beginSkillCooldownOnSkillEnd = true;
            sacrifice.canceledFromSprinting = false;
            sacrifice.fullRestockOnAssign = true;
            sacrifice.interruptPriority = InterruptPriority.Skill;
            sacrifice.isBullets = false;
            sacrifice.isCombatSkill = false;
            sacrifice.mustKeyPress = false;
            sacrifice.noSprint = false;
            sacrifice.rechargeStock = 1;
            sacrifice.requiredStock = 1;
            sacrifice.shootDelay = 0f;
            sacrifice.stockToConsume = 1;
            sacrifice.icon = Assets.icon3;
            sacrifice.skillDescriptionToken = "STANWORSHIPPER_SLOT3_UTILITY_DESCRIPTION";
            sacrifice.skillName = "STANWORSHIPPER_SLOT3_UTILITY_NAME";
            sacrifice.skillNameToken = "STANWORSHIPPER_SLOT3_UTILITY_NAME";

            //Ultimate
            var summonMajor = ScriptableObject.CreateInstance<SkillDef>();
            summonMajor.activationState = new SerializableEntityStateType(typeof(States.SummonMajor));
            summonMajor.activationStateMachineName = "Weapon";
            summonMajor.baseMaxStock = 1;
            summonMajor.baseRechargeInterval = 50f;
            summonMajor.beginSkillCooldownOnSkillEnd = true;
            summonMajor.canceledFromSprinting = false;
            summonMajor.fullRestockOnAssign = false;
            summonMajor.interruptPriority = InterruptPriority.Skill;
            summonMajor.isBullets = false;
            summonMajor.isCombatSkill = true;
            summonMajor.mustKeyPress = false;
            summonMajor.noSprint = false;
            summonMajor.rechargeStock = 1;
            summonMajor.requiredStock = 1;
            summonMajor.shootDelay = 0f;
            summonMajor.stockToConsume = 1;
            summonMajor.icon = Assets.icon4;
            summonMajor.skillDescriptionToken = "STANWORSHIPPER_SLOT4_ULTIMATE_DESCRIPTION";
            summonMajor.skillName = "STANWORSHIPPER_SLOT4_ULTIMATE_NAME";
            summonMajor.skillNameToken = "STANWORSHIPPER_SLOT4_ULTIMATE_NAME";

            LoadoutAPI.AddSkillDef(poopSlinger);
            LoadoutAPI.AddSkillDef(summonMinor);
            LoadoutAPI.AddSkillDef(sacrifice);
            LoadoutAPI.AddSkillDef(summonMajor);

            var skillLocator = stanWorshipperBody.GetComponent<SkillLocator>();

            //Primary
            var skillFamily1 = skillLocator.primary.skillFamily;
            skillFamily1.variants = new SkillFamily.Variant[1]; // substitute 1 for the number of skill variants you are implementing

            skillFamily1.variants[0] = new SkillFamily.Variant
            {
                skillDef = poopSlinger,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(poopSlinger.skillNameToken, false, null)
            };

            //Secondary
            var skillFamily2 = skillLocator.secondary.skillFamily;
            skillFamily2.variants = new SkillFamily.Variant[1]; // substitute 1 for the number of skill variants you are implementing

            skillFamily2.variants[0] = new SkillFamily.Variant
            {
                skillDef = summonMinor,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(summonMinor.skillNameToken, false, null)
            };

            //Utility
            var skillFamily3 = skillLocator.utility.skillFamily;
            skillFamily3.variants = new SkillFamily.Variant[1]; // substitute 1 for the number of skill variants you are implementing

            skillFamily3.variants[0] = new SkillFamily.Variant
            {
                skillDef = sacrifice,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(sacrifice.skillNameToken, false, null)
            };

            //Ultimate
            var skillFamily4 = skillLocator.special.skillFamily;
            skillFamily4.variants = new SkillFamily.Variant[1]; // substitute 1 for the number of skill variants you are implementing

            skillFamily4.variants[0] = new SkillFamily.Variant
            {
                skillDef = summonMajor,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(summonMajor.skillNameToken, false, null)
            };
        }

        private void InitializeBuffs()
        {
            BuffDef sacrificeBuffDef = new BuffDef
            {
                name = "Sacrifice",
                iconPath = "textures/bufficons/texMovespeedBuffIcon",
                buffColor = Color.yellow,
                canStack = false,
                eliteIndex = EliteIndex.None

            };
            CustomBuff sacrificeCustomBuff = new CustomBuff(sacrificeBuffDef);
            sacrificeBuff = BuffAPI.Add(sacrificeCustomBuff);
        }

        private void InitializeProjectiles() 
        {
            poopslingerProjectile = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/ClayPotProjectile"), "PoopslingerProjectile", true);
            if (!poopslingerProjectile.GetComponent<NetworkIdentity>()) poopslingerProjectile.AddComponent<NetworkIdentity>();
            poopslingerProjectile.GetComponent<ProjectileSimple>().velocity = 100f;
            poopslingerProjectile.GetComponent<ProjectileController>().procCoefficient = 1.5f;
            poopslingerProjectile.GetComponent<ProjectileController>().ghostPrefab = Resources.Load<GameObject>("prefabs/projectileghosts/BeetleQueenSpitGhost");
            poopslingerProjectile.GetComponent<ProjectileDamage>().damage = 1f;
            poopslingerProjectile.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
            poopslingerProjectile.GetComponentInChildren<ProjectileImpactExplosion>().blastRadius = 6.5f;
            poopslingerProjectile.GetComponentInChildren<ProjectileImpactExplosion>().impactEffect = poopslingerExplosionEffect;

            ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(poopslingerProjectile);
            };
        }

        private void InitializeSummons()
        {
            LanguageAPI.Add("WEAKSTANFRAGMENT_NAME", "Lesser Stan Fragment");
            LanguageAPI.Add("STRONGSTANFRAGMENT_NAME", "Greater Stan Fragment");

            #region Weak Stan Fragment
            weakStanFragmentMaster = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/charactermasters/JellyfishMaster"), "WeakStanFragmentMaster", true);
            weakStanFragmentBody = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/characterbodies/JellyfishBody"), "WeakStanFragmentBody", true);
            if (!weakStanFragmentMaster.GetComponent<NetworkIdentity>()) weakStanFragmentMaster.AddComponent<NetworkIdentity>();
            if (!weakStanFragmentBody.GetComponent<NetworkIdentity>()) weakStanFragmentBody.AddComponent<NetworkIdentity>();
            CharacterBody weakStanBody = weakStanFragmentBody.GetComponent<CharacterBody>();
            SkillLocator weakStanSkills = weakStanFragmentBody.GetComponent<SkillLocator>();
            var weakStanStateMachine = weakStanFragmentBody.GetComponent<EntityStateMachine>();
            weakStanBody.baseNameToken = "WEAKSTANFRAGMENT_NAME";
            weakStanBody.portraitIcon = Assets.stanFace.texture;
            weakStanBody.baseMaxHealth = 200f;
            weakStanBody.levelMaxHealth = 30f;
            weakStanBody.baseMoveSpeed = 20f;
            weakStanBody.baseAcceleration = 30f;

            //Replaces default deathstate to prevent errors.
            LoadoutAPI.AddSkill(typeof(States.SummonStates.WeakStanDeathState));
            weakStanFragmentBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(States.SummonStates.WeakStanDeathState));

            //Sets custom non-ability entitystates.
            LoadoutAPI.AddSkill(typeof(States.SummonStates.WeakStanSpawnState));
            LoadoutAPI.AddSkill(typeof(States.SummonStates.WeakStanMain));
            weakStanStateMachine.initialStateType = new SerializableEntityStateType(typeof(States.SummonStates.WeakStanSpawnState));
            weakStanStateMachine.mainStateType = new SerializableEntityStateType(typeof(States.SummonStates.WeakStanMain));

            //Disable default renderer and replace with sprite renderer for Stan sprite.
            weakStanFragmentBody.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.SetActive(false);
            SpriteRenderer weakStanSpriteRenderer = weakStanFragmentBody.AddComponent<SpriteRenderer>();
            weakStanSpriteRenderer.sprite = Assets.stanFace;

            //Adds a red light to the body.
            Light weakStanLight = weakStanFragmentBody.AddComponent<Light>();
            weakStanLight.type = LightType.Point;
            weakStanLight.range = 10f;
            weakStanLight.renderMode = LightRenderMode.Auto;
            weakStanLight.intensity = 100f;
            weakStanLight.color = Color.red;

            //Life is pain. AhaA.
            //Adds particle effects to the Minor Stan Fragments.
            foreach (ParticleSystem i in Instantiate(Resources.Load<GameObject>("prefabs/characterbodies/wispbody")).GetComponentsInChildren<ParticleSystem>())
            {
                if(i.gameObject.name == "Fire")
                {
                    wispBodyClone = i.gameObject;
                }
            }
            ParticleSystem weakStanParticleSystem = weakStanFragmentBody.AddComponent<ParticleSystem>();
            var copy1 = weakStanParticleSystem.GetCopyOf<ParticleSystem>(wispBodyClone.GetComponent<ParticleSystem>());
            var main1= weakStanParticleSystem.main;
            var colt1 = weakStanParticleSystem.colorOverLifetime;
            var solt1 = weakStanParticleSystem.sizeOverLifetime;
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKey = new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f)};
            GradientAlphaKey[] alphaKey = new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(0.15f, 0.25f), new GradientAlphaKey(0.15f, 0.75f), new GradientAlphaKey(0.0f, 1.0f) };
            gradient.SetKeys(colorKey, alphaKey);
            colt1.enabled = true;
            colt1.color = gradient;
            AnimationCurve animation = new AnimationCurve();
            animation.AddKey(0.0f, 0.0f);
            animation.AddKey(0.0f, 0.0f);
            animation.AddKey(0.5f, 0.95f);
            animation.AddKey(1.0f, 0.2f);
            solt1.enabled = true;
            solt1.size = new ParticleSystem.MinMaxCurve(2f, animation);
            main1.gravityModifier = 0f;
            weakStanParticleSystem.GetComponent<ParticleSystemRenderer>().material = Assets.weakStanFireMaterial;

            //Remove the default explode skill.
            foreach (GenericSkill i in weakStanBody.GetComponentsInChildren<GenericSkill>())
            {
                DestroyImmediate(i);
            }
            var passive = weakStanSkills.passiveSkill;
            passive.enabled = true;
            passive.skillNameToken = "";
            passive.skillDescriptionToken = "";
            passive.icon = null;
            

            //Adds the body to the character master.
            weakStanFragmentMaster.GetComponent<CharacterMaster>().bodyPrefab = weakStanFragmentBody;

            BaseAI weakStanBaseAI = weakStanFragmentMaster.GetComponent<BaseAI>();
            weakStanBaseAI.neverRetaliateFriendlies = true;
            #endregion

            #region Strong Stan Fragment
            strongStanFragmentMaster = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/charactermasters/RoboBallBossMaster"), "StrongStanFragmentMaster", true);
            strongStanFragmentBody = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/characterbodies/RoboBallBossBody"), "StrongStanFragmentBody", true);
            if (!strongStanFragmentMaster.GetComponent<NetworkIdentity>()) strongStanFragmentMaster.AddComponent<NetworkIdentity>();
            if (!strongStanFragmentBody.GetComponent<NetworkIdentity>()) strongStanFragmentBody.AddComponent<NetworkIdentity>();
            CharacterBody strongStanBody = strongStanFragmentBody.GetComponent<CharacterBody>();
            SkillLocator strongStanSkills = strongStanFragmentBody.GetComponent<SkillLocator>();
            var strongStanStateMachine = strongStanFragmentBody.GetComponent<EntityStateMachine>();
            strongStanBody.baseNameToken = "STRONGSTANFRAGMENT_NAME";
            strongStanBody.portraitIcon = Assets.stanFace2.texture;
            strongStanBody.baseMoveSpeed = 1f;

            //Remove default DeathState to prevent weird errors/unwanted effects.
            LoadoutAPI.AddSkill(typeof(States.SummonStates.StrongStanDeathState));
            strongStanFragmentBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(States.SummonStates.StrongStanDeathState));

            //Sets custom non-ability entitystates.
            LoadoutAPI.AddSkill(typeof(States.SummonStates.StrongStanSpawnState));
            LoadoutAPI.AddSkill(typeof(States.SummonStates.StrongStanMain));
            strongStanStateMachine.initialStateType = new SerializableEntityStateType(typeof(States.SummonStates.StrongStanSpawnState));
            strongStanStateMachine.mainStateType = new SerializableEntityStateType(typeof(States.SummonStates.StrongStanMain));

            //Remove default render meshes and replace with sprite mesh for strongstan.
            foreach (SkinnedMeshRenderer i in strongStanFragmentBody.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                i.gameObject.SetActive(false);
            }
            SpriteRenderer strongStanSpriteRenderer = strongStanFragmentBody.AddComponent<SpriteRenderer>();
            strongStanSpriteRenderer.sprite = Assets.stanFace2Large;

            //Add red light to strongstan.
            Light strongStanLight = strongStanFragmentBody.AddComponent<Light>();
            strongStanLight.type = LightType.Point;
            strongStanLight.range = 8f;
            strongStanLight.renderMode = LightRenderMode.Auto;
            strongStanLight.intensity = 75f;
            strongStanLight.color = Color.red;

            //Add the same particle system as weakstan but with bigger particles (looks like shit lmao).
            ParticleSystem strongStanParticleSystem = strongStanFragmentBody.AddComponent<ParticleSystem>();
            var copy2 = strongStanParticleSystem.GetCopyOf<ParticleSystem>(wispBodyClone.GetComponent<ParticleSystem>());
            var main2 = strongStanParticleSystem.main;
            var colt2 = strongStanParticleSystem.colorOverLifetime;
            var solt2 = strongStanParticleSystem.sizeOverLifetime;
            colt2.enabled = true;
            colt2.color = gradient;
            solt2.enabled = true;
            solt2.size = new ParticleSystem.MinMaxCurve(7f, animation);
            main2.gravityModifier = 0f;
            strongStanParticleSystem.GetComponent<ParticleSystemRenderer>().material = Assets.weakStanFireMaterial;

            //Remove the solus boss templates default utility(spawn probes) and ultimate(aoe slow rape).
            foreach (GenericSkill i in strongStanBody.GetComponentsInChildren<GenericSkill>())
            {
                DestroyImmediate(i);
            }
            //Define the strong stan fragmen's skilldef and add it to him
            //I wanted to do this in the IntializeSkills function but im too retarded to figure out how the type var works and using the type skilldef doesnt work
            //Basically: disgusting :)
            var chargePoopBlast = ScriptableObject.CreateInstance<SkillDef>();
            chargePoopBlast.activationState = new SerializableEntityStateType(typeof(States.SummonStates.ChargePoopBlast));
            chargePoopBlast.activationStateMachineName = "Weapon";
            chargePoopBlast.baseMaxStock = 1;
            chargePoopBlast.baseRechargeInterval = 0f;
            chargePoopBlast.beginSkillCooldownOnSkillEnd = true;
            chargePoopBlast.canceledFromSprinting = false;
            chargePoopBlast.fullRestockOnAssign = true;
            chargePoopBlast.interruptPriority = InterruptPriority.Any;
            chargePoopBlast.isBullets = false;
            chargePoopBlast.isCombatSkill = true;
            chargePoopBlast.mustKeyPress = false;
            chargePoopBlast.noSprint = true;
            chargePoopBlast.rechargeStock = 1;
            chargePoopBlast.requiredStock = 1;
            chargePoopBlast.shootDelay = 0f;
            chargePoopBlast.stockToConsume = 1;
            chargePoopBlast.icon = null;
            chargePoopBlast.skillDescriptionToken = "";
            chargePoopBlast.skillName = "";
            chargePoopBlast.skillNameToken = "";
            LoadoutAPI.AddSkillDef(chargePoopBlast);
            strongStanSkills.primary = strongStanFragmentBody.AddComponent<GenericSkill>();
            SkillFamily newFamily2 = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily2.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily2);
            strongStanSkills.primary.SetFieldValue("_skillFamily", newFamily2);
            SkillFamily skillFamily2 = strongStanSkills.primary.skillFamily;
            skillFamily2.variants[0] = new SkillFamily.Variant
            {
                skillDef = chargePoopBlast,
                unlockableName = "",
            };

            //Actually adds the body defined above to the strongstan character master.
            strongStanFragmentMaster.GetComponent<CharacterMaster>().bodyPrefab = strongStanFragmentBody;

            BaseAI strongStanBaseAI = strongStanFragmentMaster.GetComponent<BaseAI>();
            strongStanBaseAI.aimVectorMaxSpeed = 500f;
            strongStanBaseAI.aimVectorDampTime = 0.1f;
            strongStanBaseAI.enemyAttentionDuration = 0.25f;
            strongStanBaseAI.minDistanceFromEnemy = 0f;
            strongStanBaseAI.fullVision = true;
            strongStanBaseAI.neverRetaliateFriendlies = true;
            #endregion

            BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(strongStanFragmentBody);
                list.Add(weakStanFragmentBody);
            };
            MasterCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(strongStanFragmentMaster);
                list.Add(weakStanFragmentMaster);
            };
        }

        private void InitializeINetMessages()
        {
            NetworkingAPI.RegisterMessageType<NetMessages.SoundMessage>();
            NetworkingAPI.RegisterMessageType<NetMessages.SpawnStanMinonMessage>();
            NetworkingAPI.RegisterMessageType<NetMessages.HealFractionMessage>();
            NetworkingAPI.RegisterMessageType<NetMessages.TimedBuffMessage>();
        }

        private void CreateDoppelganger()
        {

            stanWorshipperDoppelganger = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "StanWorshipperMonsterMaster", true);
            if (!stanWorshipperDoppelganger.GetComponent<NetworkIdentity>()) stanWorshipperDoppelganger.AddComponent<NetworkIdentity>();

            MasterCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(stanWorshipperDoppelganger);
            };

            CharacterMaster component = stanWorshipperDoppelganger.GetComponent<CharacterMaster>();
            component.bodyPrefab = stanWorshipperBody;

        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self)
            {
                if (self.HasBuff(sacrificeBuff))
                {
                    self.moveSpeed += self.moveSpeed * States.Sacrifice.moveSpeedBoost;
                }
            }
        }
    }
}