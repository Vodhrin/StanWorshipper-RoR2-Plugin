using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Object;
using UnityEngine.Networking;
using RoR2;
using RoR2.Skills;
using RoR2.CharacterAI;
using R2API;
using R2API.Utils;
using EntityStates;
using StanWorshipper.Core;

namespace StanWorshipper.Survivor
{
    internal static class Summons
    {
        public static GameObject weakStanFragmentMaster;
        public static GameObject weakStanFragmentBody;

        public static GameObject strongStanFragmentMaster;
        public static GameObject strongStanFragmentBody;

        public static void Initialize()
        {
            #region Weak Stan Fragment
            weakStanFragmentMaster = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/charactermasters/JellyfishMaster"), "WeakStanFragmentMaster", true);
            weakStanFragmentBody = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/characterbodies/JellyfishBody"), "WeakStanFragmentBody", true);
            if (!weakStanFragmentMaster.GetComponent<NetworkIdentity>()) weakStanFragmentMaster.AddComponent<NetworkIdentity>();
            if (!weakStanFragmentBody.GetComponent<NetworkIdentity>()) weakStanFragmentBody.AddComponent<NetworkIdentity>();
            var weakStanBody = weakStanFragmentBody.GetComponent<CharacterBody>();
            var weakStanSkills = weakStanFragmentBody.GetComponent<SkillLocator>();
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
            var weakStanSpriteRenderer = weakStanFragmentBody.AddComponent<SpriteRenderer>();
            weakStanSpriteRenderer.sprite = Assets.stanFace;

            //Adds a red light to the body.
            var weakStanLight = weakStanFragmentBody.AddComponent<Light>();
            weakStanLight.type = LightType.Point;
            weakStanLight.range = 10f;
            weakStanLight.renderMode = LightRenderMode.Auto;
            weakStanLight.intensity = 100f;
            weakStanLight.color = Color.red;

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

            var weakStanBaseAI = weakStanFragmentMaster.GetComponent<BaseAI>();
            weakStanBaseAI.neverRetaliateFriendlies = true;
            #endregion

            #region Strong Stan Fragment
            strongStanFragmentMaster = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/charactermasters/RoboBallBossMaster"), "StrongStanFragmentMaster", true);
            strongStanFragmentBody = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/characterbodies/RoboBallBossBody"), "StrongStanFragmentBody", true);
            if (!strongStanFragmentMaster.GetComponent<NetworkIdentity>()) strongStanFragmentMaster.AddComponent<NetworkIdentity>();
            if (!strongStanFragmentBody.GetComponent<NetworkIdentity>()) strongStanFragmentBody.AddComponent<NetworkIdentity>();
            var strongStanBody = strongStanFragmentBody.GetComponent<CharacterBody>();
            var strongStanSkills = strongStanFragmentBody.GetComponent<SkillLocator>();
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
            var strongStanSpriteRenderer = strongStanFragmentBody.AddComponent<SpriteRenderer>();
            strongStanSpriteRenderer.sprite = Assets.stanFace2Large;

            //Add red light to strongstan.
            var strongStanLight = strongStanFragmentBody.AddComponent<Light>();
            strongStanLight.type = LightType.Point;
            strongStanLight.range = 8f;
            strongStanLight.renderMode = LightRenderMode.Auto;
            strongStanLight.intensity = 75f;
            strongStanLight.color = Color.red;

            //Remove the solus boss templates default utility(spawn probes) and ultimate(aoe slow rape).
            foreach (GenericSkill i in strongStanBody.GetComponentsInChildren<GenericSkill>())
            {
                DestroyImmediate(i);
            }
            //Define the strong stan fragmen's skilldef and add it to him

            var chargePoopBlast = ScriptableObject.CreateInstance<SkillDef>();
            chargePoopBlast.activationState = new SerializableEntityStateType(typeof(States.SummonStates.ChargePoopBlast));
            chargePoopBlast.activationStateMachineName = "Weapon";
            chargePoopBlast.baseMaxStock = 1;
            chargePoopBlast.baseRechargeInterval = 0f;
            chargePoopBlast.beginSkillCooldownOnSkillEnd = true;
            chargePoopBlast.canceledFromSprinting = false;
            chargePoopBlast.fullRestockOnAssign = true;
            chargePoopBlast.interruptPriority = InterruptPriority.Any;
            chargePoopBlast.isCombatSkill = true;
            chargePoopBlast.mustKeyPress = false;
            chargePoopBlast.cancelSprintingOnActivation = true;
            chargePoopBlast.rechargeStock = 1;
            chargePoopBlast.requiredStock = 1;
            chargePoopBlast.stockToConsume = 1;
            chargePoopBlast.icon = null;
            chargePoopBlast.skillDescriptionToken = "";
            chargePoopBlast.skillName = "";
            chargePoopBlast.skillNameToken = "";
            LoadoutAPI.AddSkillDef(chargePoopBlast);
            StanWorshipperPlugin.skillDefs.Add(chargePoopBlast);
            strongStanSkills.primary = strongStanFragmentBody.AddComponent<GenericSkill>();
            var newFamily2 = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily2.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily2);
            strongStanSkills.primary.SetFieldValue("_skillFamily", newFamily2);
            var skillFamily2 = strongStanSkills.primary.skillFamily;
            skillFamily2.variants[0] = new SkillFamily.Variant
            {
                skillDef = chargePoopBlast,
            };

            //Actually adds the body defined above to the strongstan character master.
            strongStanFragmentMaster.GetComponent<CharacterMaster>().bodyPrefab = strongStanFragmentBody;

            var strongStanBaseAI = strongStanFragmentMaster.GetComponent<BaseAI>();
            strongStanBaseAI.aimVectorMaxSpeed = 500f;
            strongStanBaseAI.aimVectorDampTime = 0.1f;
            strongStanBaseAI.enemyAttentionDuration = 0.25f;
            strongStanBaseAI.fullVision = true;
            strongStanBaseAI.neverRetaliateFriendlies = true;
            #endregion

            StanWorshipperPlugin.characterBodies.Add(strongStanFragmentBody);
            StanWorshipperPlugin.characterBodies.Add(weakStanFragmentBody);
            StanWorshipperPlugin.characterMasters.Add(strongStanFragmentMaster);
            StanWorshipperPlugin.characterMasters.Add(weakStanFragmentMaster);
        }
    }
}
