using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using R2API;
using EntityStates;
using HG;
using StanWorshipper.Core;

namespace StanWorshipper.Survivor
{
    internal static class Character
    {
        public static GameObject stanWorshipperBody;
        public static GameObject stanWorshipperDisplay;
        public static GameObject stanWorshipperDoppelganger;

        public static void Create()
        {
            stanWorshipperBody = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/characterbodies/BanditBody"), "StanWorshipperBody", true);
            stanWorshipperBody.GetComponent<NetworkIdentity>().localPlayerAuthority = true;

            CharacterBody bodyComponent = stanWorshipperBody.GetComponent<CharacterBody>();
            bodyComponent.bodyIndex = BodyIndex.None;
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
                RootObject = model
            };
            SkinDef skin = LoadoutAPI.CreateNewSkinDef(skinDefInfo);
            skinController.skins = new SkinDef[] { skin };

            //Doppelganger
            stanWorshipperDoppelganger = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "StanWorshipperMonsterMaster", true);
            if (!stanWorshipperDoppelganger.GetComponent<NetworkIdentity>()) stanWorshipperDoppelganger.AddComponent<NetworkIdentity>();

            CharacterMaster component = stanWorshipperDoppelganger.GetComponent<CharacterMaster>();
            component.bodyPrefab = stanWorshipperBody;
        }

        public static void Initialize()
        {
            stanWorshipperDisplay = PrefabAPI.InstantiateClone(stanWorshipperBody.GetComponent<ModelLocator>().modelBaseTransform.gameObject, "StanWorshipperDisplay", true);
            stanWorshipperDisplay.AddComponent<NetworkIdentity>();

            var stanWorshipper = ScriptableObject.CreateInstance<SurvivorDef>();

            stanWorshipper.bodyPrefab = stanWorshipperBody;
            stanWorshipper.cachedName = "STANWORSHIPPER_NAME";
            stanWorshipper.displayNameToken = "STANWORSHIPPER_NAME";
            stanWorshipper.descriptionToken = "STANWORSHIPPER_DESCRIPTION";
            stanWorshipper.outroFlavorToken = "STANWORSHIPPER_OUTRO_FLAVOR";
            stanWorshipper.displayPrefab = stanWorshipperDisplay;
            stanWorshipper.primaryColor = new Color(0.6f, 0.3f, 0f);

            SurvivorAPI.AddSurvivor(stanWorshipper);

            //Unsure if R2API does this automatically.
            StanWorshipperPlugin.characterMasters.Add(stanWorshipperDoppelganger);
        }
    }
}
