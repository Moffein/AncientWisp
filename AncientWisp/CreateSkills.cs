using EntityStates;
using EntityStates.MoffeinAncientWispSkills;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;

namespace AncientWisp
{
    public class CreateSkills
    {
        public CreateSkills(SkillLocator skills)
        {
            AssignLightning(skills);
            AssignBarrage(skills);
            AssignUtility(skills);
        }
        private void AssignLightning(SkillLocator skills)
        {
            SkillFamily barrageSkillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            barrageSkillFamily.defaultVariantIndex = 0u;
            barrageSkillFamily.variants = new SkillFamily.Variant[1];
            skills.secondary._skillFamily = barrageSkillFamily;

            SkillDef barrageDef = SkillDef.CreateInstance<SkillDef>();
            barrageDef.activationState = new SerializableEntityStateType(typeof(EntityStates.MoffeinAncientWispSkills.ChargeBarrage));
            barrageDef.activationStateMachineName = "Weapon";
            barrageDef.baseMaxStock = 2;
            barrageDef.baseRechargeInterval = 7f;
            barrageDef.beginSkillCooldownOnSkillEnd = true;
            barrageDef.canceledFromSprinting = false;
            barrageDef.dontAllowPastMaxStocks = false;
            barrageDef.forceSprintDuringState = false;
            barrageDef.fullRestockOnAssign = true;
            barrageDef.icon = null;
            barrageDef.interruptPriority = InterruptPriority.Any;
            barrageDef.isCombatSkill = true;
            barrageDef.keywordTokens = new string[] { };
            barrageDef.mustKeyPress = false;
            barrageDef.cancelSprintingOnActivation = false;
            barrageDef.rechargeStock = 2;
            barrageDef.requiredStock = 1;
            barrageDef.skillName = "FireBarrage";
            barrageDef.skillNameToken = "FireBarrage";
            barrageDef.skillDescriptionToken = "";
            barrageDef.stockToConsume = 1;
            (barrageDef as UnityEngine.Object).name = barrageDef.name;
            AWContent.entityStates.Add(typeof(ChargeBarrage));
            AWContent.entityStates.Add(typeof(FireBarrage));
            AWContent.skillDefs.Add(barrageDef);
            AWContent.skillFamilies.Add(barrageSkillFamily);
            barrageSkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = barrageDef,
                viewableNode = new ViewablesCatalog.Node(barrageDef.skillNameToken, false)
            };
        }

        private void AssignBarrage(SkillLocator skills)
        {
            SkillFamily lightningSkillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            lightningSkillFamily.defaultVariantIndex = 0u;
            lightningSkillFamily.variants = new SkillFamily.Variant[1];
            skills.primary._skillFamily = lightningSkillFamily;

            SkillDef rainDef = SkillDef.CreateInstance<SkillDef>();
            rainDef.activationState = new SerializableEntityStateType(typeof(EntityStates.MoffeinAncientWispSkills.ChargeRain));
            rainDef.activationStateMachineName = "Weapon";
            rainDef.baseMaxStock = 1;
            rainDef.baseRechargeInterval = 22f;
            rainDef.beginSkillCooldownOnSkillEnd = false;
            rainDef.canceledFromSprinting = false;
            rainDef.dontAllowPastMaxStocks = false;
            rainDef.forceSprintDuringState = false;
            rainDef.fullRestockOnAssign = true;
            rainDef.icon = null;
            rainDef.interruptPriority = InterruptPriority.Any;
            rainDef.isCombatSkill = false;
            rainDef.keywordTokens = new string[] { };
            rainDef.mustKeyPress = false;
            rainDef.cancelSprintingOnActivation = false;
            rainDef.rechargeStock = 1;
            rainDef.requiredStock = 1;
            rainDef.skillName = "ChargeRain";
            rainDef.skillNameToken = "Rain";
            rainDef.skillDescriptionToken = "";
            rainDef.stockToConsume = 1;
            (rainDef as UnityEngine.Object).name = rainDef.name;
            lightningSkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = rainDef,
                viewableNode = new ViewablesCatalog.Node(rainDef.skillNameToken, false)
            };
            AWContent.entityStates.Add(typeof(ChargeRain));
            AWContent.entityStates.Add(typeof(ChannelRain));
            AWContent.entityStates.Add(typeof(EndRain));
            AWContent.skillDefs.Add(rainDef);
            AWContent.skillFamilies.Add(lightningSkillFamily);
        }
        private void AssignUtility(SkillLocator skills)
        {
            SkillFamily utilitySkillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            utilitySkillFamily.defaultVariantIndex = 0u;
            utilitySkillFamily.variants = new SkillFamily.Variant[1];
            skills.utility._skillFamily = utilitySkillFamily;

            SkillDef enrageDef = SkillDef.CreateInstance<SkillDef>();
            enrageDef.activationState = new SerializableEntityStateType(typeof(EntityStates.MoffeinAncientWispSkills.Enrage));
            enrageDef.activationStateMachineName = "Weapon";
            enrageDef.baseMaxStock = 1;
            enrageDef.baseRechargeInterval = 0f;
            enrageDef.beginSkillCooldownOnSkillEnd = false;
            enrageDef.canceledFromSprinting = false;
            enrageDef.dontAllowPastMaxStocks = false;
            enrageDef.forceSprintDuringState = false;
            enrageDef.fullRestockOnAssign = true;
            enrageDef.icon = null;
            enrageDef.interruptPriority = InterruptPriority.Skill;
            enrageDef.isCombatSkill = false;
            enrageDef.keywordTokens = new string[] { };
            enrageDef.mustKeyPress = false;
            enrageDef.cancelSprintingOnActivation = false;
            enrageDef.rechargeStock = 0;
            enrageDef.requiredStock = 1;
            enrageDef.skillName = "Enrage";
            enrageDef.skillNameToken = "Enrage";
            enrageDef.skillDescriptionToken = "";
            enrageDef.stockToConsume = 1;
            utilitySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = enrageDef,
                viewableNode = new ViewablesCatalog.Node(enrageDef.skillNameToken, false)
            };

            (enrageDef as UnityEngine.Object).name = enrageDef.name;

            AWContent.entityStates.Add(typeof(Enrage));
            AWContent.skillDefs.Add(enrageDef);
            AWContent.skillFamilies.Add(utilitySkillFamily);
        }
    }
}
