using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using RoR2.Skills;
using BepInEx;
using R2API.Utils;


namespace StanWorshipper
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin("com.Vodhr.StanWorshipperSurvivor", "Stan Worshipper Survivor", "0.0.4")]
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
        public static List<GameObject> characterBodies;
        public static List<GameObject> characterMasters;
        public static List<BuffDef> buffDefs;
        public static List<EffectDef> effectDefs;
        public static List<Type> entityStateTypes;
        public static List<NetworkSoundEventDef> networkSoundEventDefs;
        public static List<GameObject> projectilePrefabs;
        public static List<SkillDef> skillDefs;
        public static List<SkillFamily> skillFamilies;
        public static List<SurvivorDef> survivorDefs;
        public static List<UnlockableDef> unlockableDefs;

        public static BepInEx.Logging.ManualLogSource logger;

        public void Awake()
        {
            InitializeLists();
            CreateHooks();

            Core.Assets.Initialize();
            Core.Language.Initialize();
            Survivor.Character.Create();
            Survivor.Character.Initialize();
            Survivor.Effects.Initialize();
            Survivor.Skills.Initialize();
            Survivor.Buffs.Initialize();
            Survivor.Projectiles.Initialize();
            Survivor.Summons.Initialize();
            Core.NetMessages.Initialize();

            new Core.StanWorshipperContentPack().Initialize();

            logger = this.Logger;
        }

        private static void InitializeLists()
        {
            characterBodies = new List<GameObject>();
            characterMasters = new List<GameObject>();
            buffDefs = new List<BuffDef>();
            effectDefs = new List<EffectDef>();
            entityStateTypes = new List<Type>();
            networkSoundEventDefs = new List<NetworkSoundEventDef>();
            projectilePrefabs = new List<GameObject>();
            skillDefs = new List<SkillDef>();
            skillFamilies = new List<SkillFamily>();
            survivorDefs = new List<SurvivorDef>();
            unlockableDefs = new List<UnlockableDef>();
        }

        private void CreateHooks()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.BuffCatalog.Init += BuffCatalog_Init;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self)
            {
                if (self.HasBuff(Survivor.Buffs.sacrificeBuff))
                {
                    self.moveSpeed += self.moveSpeed * Survivor.States.Sacrifice.moveSpeedBoost;
                }
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {

            if (damageInfo == null) { orig(self, damageInfo); return; }

            CharacterBody characterBody = self.gameObject.GetComponent<CharacterBody>();
            CharacterBody attackerCharacterBody = damageInfo.attacker.GetComponent<CharacterBody>();

            if (attackerCharacterBody.baseNameToken == "STRONGSTANFRAGMENT_NAME")
            {
                damageInfo.attacker = attackerCharacterBody.master.minionOwnership.ownerMaster.GetBody().gameObject;
                logger.LogMessage(attackerCharacterBody.master.minionOwnership.ownerMaster.GetBody().gameObject);
            }
            orig(self, damageInfo);
        }

        private void BuffCatalog_Init(On.RoR2.BuffCatalog.orig_Init orig)
        {
            orig();
            Survivor.Buffs.GetBuffIndices();
        }
    }
}
