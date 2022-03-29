using UnityEngine;
using RoR2.CharacterAI;
using RoR2;

namespace AncientWisp
{
    class CreateAI
    {
        public CreateAI(GameObject masterObject)
        {
            AISkillDriver rage = masterObject.AddComponent<AISkillDriver>();
            rage.skillSlot = SkillSlot.Utility;
            rage.requireSkillReady = true;
            rage.requireEquipmentReady = false;
            rage.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            rage.minDistance = 0f;
            rage.maxDistance = float.PositiveInfinity;
            rage.selectionRequiresTargetLoS = false;
            rage.activationRequiresTargetLoS = false;
            rage.activationRequiresAimConfirmation = false;
            rage.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            rage.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            rage.ignoreNodeGraph = false;
            rage.noRepeat = true;
            rage.shouldSprint = false;
            rage.shouldFireEquipment = false;
            rage.shouldTapButton = false;
            rage.maxUserHealthFraction = 0.5f;
            rage.minUserHealthFraction = 0f;

            AISkillDriver rain = masterObject.AddComponent<AISkillDriver>();
            rain.skillSlot = SkillSlot.Primary;
            rain.requireSkillReady = true;
            rain.requireEquipmentReady = false;
            rain.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            rain.minDistance = 0f;
            rain.maxDistance = 150f;
            rain.selectionRequiresTargetLoS = true;
            rain.activationRequiresTargetLoS = false;
            rain.activationRequiresAimConfirmation = false;
            rain.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            rain.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            rain.ignoreNodeGraph = false;
            rain.noRepeat = true;
            rain.shouldSprint = false;
            rain.shouldFireEquipment = false;
            rain.shouldTapButton = false;
            rain.maxUserHealthFraction = 1f;

            AISkillDriver chasePriority = masterObject.AddComponent<AISkillDriver>();
            chasePriority.skillSlot = SkillSlot.None;
            chasePriority.requireSkillReady = false;
            chasePriority.requireEquipmentReady = false;
            chasePriority.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chasePriority.minDistance = 150f;
            chasePriority.maxDistance = float.PositiveInfinity;
            chasePriority.selectionRequiresTargetLoS = false;
            chasePriority.activationRequiresTargetLoS = false;
            chasePriority.activationRequiresAimConfirmation = false;
            chasePriority.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chasePriority.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            chasePriority.ignoreNodeGraph = false;
            chasePriority.noRepeat = false;
            chasePriority.shouldSprint = false;
            chasePriority.shouldFireEquipment = false;
            chasePriority.shouldTapButton = false;

            AISkillDriver barrageChase = masterObject.AddComponent<AISkillDriver>();
            barrageChase.skillSlot = SkillSlot.Secondary;
            barrageChase.requireSkillReady = true;
            barrageChase.requireEquipmentReady = false;
            barrageChase.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            barrageChase.minDistance = 50f;
            barrageChase.maxDistance = 150f;
            barrageChase.selectionRequiresTargetLoS = true;
            barrageChase.activationRequiresTargetLoS = true;
            barrageChase.activationRequiresAimConfirmation = true;
            barrageChase.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            barrageChase.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            barrageChase.driverUpdateTimerOverride = 1f;
            barrageChase.ignoreNodeGraph = false;
            barrageChase.noRepeat = false;
            barrageChase.shouldSprint = false;
            barrageChase.shouldFireEquipment = false;
            barrageChase.shouldTapButton = false;

            AISkillDriver barrage = masterObject.AddComponent<AISkillDriver>();
            barrage.skillSlot = SkillSlot.Secondary;
            barrage.requireSkillReady = true;
            barrage.requireEquipmentReady = false;
            barrage.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            barrage.minDistance = 0f;
            barrage.maxDistance = 50f;
            barrage.selectionRequiresTargetLoS = true;
            barrage.activationRequiresTargetLoS = true;
            barrage.activationRequiresAimConfirmation = true;
            barrage.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            barrage.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            barrage.driverUpdateTimerOverride = 1f;
            barrage.ignoreNodeGraph = false;
            barrage.noRepeat = false;
            barrage.shouldSprint = false;
            barrage.shouldFireEquipment = false;
            barrage.shouldTapButton = false;

            AISkillDriver chase = masterObject.AddComponent<AISkillDriver>();
            chase.skillSlot = SkillSlot.None;
            chase.requireSkillReady = false;
            chase.requireEquipmentReady = false;
            chase.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chase.minDistance = 0f;
            chase.maxDistance = float.PositiveInfinity;
            chase.selectionRequiresTargetLoS = false;
            chase.activationRequiresTargetLoS = false;
            chase.activationRequiresAimConfirmation = false;
            chase.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chase.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            chase.ignoreNodeGraph = false;
            chase.noRepeat = false;
            chase.shouldSprint = false;
            chase.shouldFireEquipment = false;
            chase.shouldTapButton = false;
        }
    }
}
