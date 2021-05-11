using UnityEngine;
using RoR2;
using RoR2.Skills;
using R2API;
using EntityStates;
using StanWorshipper.Core;

namespace StanWorshipper.Survivor
{
    internal static class Skills
    {
        public static void Initialize()
        {

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
            poopSlinger.isCombatSkill = true;
            poopSlinger.mustKeyPress = false;
            poopSlinger.rechargeStock = 1;
            poopSlinger.requiredStock = 1;
            poopSlinger.canceledFromSprinting = false;
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
            summonMinor.isCombatSkill = true;
            summonMinor.mustKeyPress = false;
            summonMinor.cancelSprintingOnActivation = false;
            summonMinor.rechargeStock = 1;
            summonMinor.requiredStock = 1;
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
            sacrifice.isCombatSkill = false;
            sacrifice.mustKeyPress = false;
            sacrifice.cancelSprintingOnActivation = false;
            sacrifice.rechargeStock = 1;
            sacrifice.requiredStock = 1;
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
            summonMajor.isCombatSkill = true;
            summonMajor.mustKeyPress = false;
            summonMajor.cancelSprintingOnActivation = false;
            summonMajor.rechargeStock = 1;
            summonMajor.requiredStock = 1;
            summonMajor.stockToConsume = 1;
            summonMajor.icon = Assets.icon4;
            summonMajor.skillDescriptionToken = "STANWORSHIPPER_SLOT4_ULTIMATE_DESCRIPTION";
            summonMajor.skillName = "STANWORSHIPPER_SLOT4_ULTIMATE_NAME";
            summonMajor.skillNameToken = "STANWORSHIPPER_SLOT4_ULTIMATE_NAME";

            LoadoutAPI.AddSkillDef(poopSlinger);
            LoadoutAPI.AddSkillDef(summonMinor);
            LoadoutAPI.AddSkillDef(sacrifice);
            LoadoutAPI.AddSkillDef(summonMajor);
            StanWorshipperPlugin.skillDefs.Add(poopSlinger);
            StanWorshipperPlugin.skillDefs.Add(summonMinor);
            StanWorshipperPlugin.skillDefs.Add(sacrifice);
            StanWorshipperPlugin.skillDefs.Add(summonMajor);
            StanWorshipperPlugin.entityStateTypes.Add(typeof(States.Poopslinger));
            StanWorshipperPlugin.entityStateTypes.Add(typeof(States.SummonMinor));
            StanWorshipperPlugin.entityStateTypes.Add(typeof(States.Sacrifice));
            StanWorshipperPlugin.entityStateTypes.Add(typeof(States.SummonMajor));

            var skillLocator = Character.stanWorshipperBody.GetComponent<SkillLocator>();

            //Primary
            var skillFamily1 = skillLocator.primary.skillFamily;
            skillFamily1.variants = new SkillFamily.Variant[1]; // substitute 1 for the number of skill variants you are implementing

            skillFamily1.variants[0] = new SkillFamily.Variant
            {
                skillDef = poopSlinger,
                viewableNode = new ViewablesCatalog.Node(poopSlinger.skillNameToken, false, null)
            };
            StanWorshipperPlugin.skillFamilies.Add(skillFamily1);

            //Secondary
            var skillFamily2 = skillLocator.secondary.skillFamily;
            skillFamily2.variants = new SkillFamily.Variant[1]; // substitute 1 for the number of skill variants you are implementing

            skillFamily2.variants[0] = new SkillFamily.Variant
            {
                skillDef = summonMinor,
                viewableNode = new ViewablesCatalog.Node(summonMinor.skillNameToken, false, null)
            };
            StanWorshipperPlugin.skillFamilies.Add(skillFamily2);

            //Utility
            var skillFamily3 = skillLocator.utility.skillFamily;
            skillFamily3.variants = new SkillFamily.Variant[1]; // substitute 1 for the number of skill variants you are implementing

            skillFamily3.variants[0] = new SkillFamily.Variant
            {
                skillDef = sacrifice,
                viewableNode = new ViewablesCatalog.Node(sacrifice.skillNameToken, false, null)
            };
            StanWorshipperPlugin.skillFamilies.Add(skillFamily3);

            //Ultimate
            var skillFamily4 = skillLocator.special.skillFamily;
            skillFamily4.variants = new SkillFamily.Variant[1]; // substitute 1 for the number of skill variants you are implementing

            skillFamily4.variants[0] = new SkillFamily.Variant
            {
                skillDef = summonMajor,
                viewableNode = new ViewablesCatalog.Node(summonMajor.skillNameToken, false, null)
            };
            StanWorshipperPlugin.skillFamilies.Add(skillFamily4);
        }
    }
}
