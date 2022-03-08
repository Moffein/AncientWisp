using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using System;
using UnityEngine;
using RoR2.Navigation;
using System.Collections.Generic;
using RoR2.Projectile;
using System.Reflection;
using AncientWisp.Helpers;
using EntityStates.MoffeinAncientWispSkills;
using R2API;
using R2API.Utils;
using MonoMod.RuntimeDetour;
using RoR2.ContentManagement;
using System.Runtime.CompilerServices;

namespace AncientWisp
{
    [BepInDependency("com.Moffein.RiskyArtifacts", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Moffein.AncientWisp", "AncientWisp", "1.4.0")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(DirectorAPI), nameof(PrefabAPI), nameof(LanguageAPI), nameof(SoundAPI), nameof(RecalculateStatsAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class AncientWisp : BaseUnityPlugin
    {
        public static float flareSize;
        public static int titanicPlains;
        public static int distantRoost;
        public static int wetlands;
        public static int aqueduct;
        public static int rallypoint;
        public static int scorched;
        public static int abyss;
        public static int sirens;
        public static int stadia;
        public static int skymeadow;
        public static int voidCell;
        public static int gilded;
        public static int artifact;

        public static int snowyForest, aphSanct, sulfur;

        public static bool allowOrigin = true;

        public static BuffDef enrageBuff;

        GameObject ancientWispObject;

        public static DirectorAPI.DirectorCardHolder ancientWispCard;

        public void Start()
        {
            DisplaySetup(ancientWispObject);
        }

        private static Type GetTypeFromName(String name)
        {
            Type[] types = EntityStateCatalog.stateIndexToType;
            return Type.GetType(name);
        }
        public void Awake()
        {
            FixEntityStates.RunFix();
            flareSize = base.Config.Bind<float>(new ConfigDefinition("00 - General", "Eye Flare Size"), 0.5f, new ConfigDescription("How big the flare effect on the eye should be. 0 disables.")).Value;
            allowOrigin = base.Config.Bind<bool>(new ConfigDefinition("00 - General", "Add to Artifact of Origination Spawnpool"), true, new ConfigDescription("Allows this boss to spawn when Origination from Risky Artifacts is enabled.")).Value;
            titanicPlains = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Titanic Plains"), -1, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            distantRoost = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Distant Roost"), -1, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            wetlands = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Wetland Aspect"), -1, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            aqueduct = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Abandoned Aqueduct"), -1, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            rallypoint = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Rallypoint Delta"), -1, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            scorched = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Scorched Acres"), -1, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            abyss = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Abyssal Depths"), 0, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            sirens = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Sirens Call"), -1, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            stadia = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Sundered Grove"), 0, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            skymeadow = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Sky Meadow"), 0, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            voidCell = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Void Fields"), -1, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            artifact = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Bulwarks Ambry"), 0, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            gilded = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Gilded Coast"), 0, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            snowyForest = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Siphoned Forest"), -1, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            aphSanct = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Aphelian Sanctuary"), -1, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;
            sulfur = base.Config.Bind<int>(new ConfigDefinition("01 - Stages", "Sulfur Pools"), 5, new ConfigDescription("Minimum stage completions before the boss can spawn. -1 = disabled, 0 = can spawn anytime, 5 = loop-only")).Value;

            RegisterLanguageTokens();
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AncientWisp.moffeinancientwisp"))
            {
                AWContent.assets = AssetBundle.LoadFromStream(stream);
            }

            using (var bankStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AncientWisp.AncientWispBank.bnk"))
            {
                var bytes = new byte[bankStream.Length];
                bankStream.Read(bytes, 0, bytes.Length);
                SoundAPI.SoundBanks.Add(bytes);
            }

            RepairEffects.Repair();

            ancientWispObject = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/AncientWispBody"), "MoffeinAncientWispBody", true);
            GameObject ancientWispMaster = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/AncientWispMaster"), "MoffeinAncientWispMaster", true);
            CharacterBody ancientWispBody = ancientWispObject.GetComponent<CharacterBody>();
            SkillLocator ancientWispSkills = ancientWispObject.GetComponent<SkillLocator>();


            ancientWispObject.GetComponent<CameraTargetParams>().cameraParams = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/ImpBossBody").GetComponent<CameraTargetParams>().cameraParams;

            ancientWispMaster.GetComponent<CharacterMaster>().bodyPrefab = ancientWispObject;

            DeathRewards deathReward = ancientWispObject.GetComponent<DeathRewards>();
            deathReward.bossPickup = new SerializablePickupIndex()
            {
                pickupName = "ItemIndex.SprintWisp"
            };

            SfxLocator ancientWispSfx = ancientWispObject.AddComponent<SfxLocator>();
            ancientWispSfx.barkSound = "Play_magmaWorm_idle_VO";
            ancientWispSfx.deathSound = "Play_MoffeinAW_death";


            SetBodyStats(ancientWispBody);
            DirectorActions(ancientWispMaster);
            ModifyArchWispProjectile();
            BuildLightningProjectile();
            FixHitbox(ancientWispObject);

            AssignSkills(ancientWispSkills);
            BuildAI(ancientWispMaster);


            AWContent.masterPrefabs.Add(ancientWispMaster);
            AWContent.bodyPrefabs.Add(ancientWispObject);

            Interactor interactor = ancientWispObject.GetComponent<Interactor>();
            interactor.maxInteractionDistance = 6f;

            enrageBuff = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/EnrageAncientWisp");

            On.RoR2.CharacterBody.OnDeathStart += (orig, self) =>
            {
                orig(self);
                if (self.baseNameToken == "MOFFEIN_ANCIENTWISP_BODY_NAME")
                {
                    self.gameObject.AddComponent<DeathExplosions>();
                }
            };

            RecalculateStatsAPI.GetStatCoefficients += (CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args) =>
            {
                if (sender.HasBuff(enrageBuff))
                {
                    args.attackSpeedMultAdd += 0.5f;
                    args.moveSpeedMultAdd += 0.3f;
                }
            };

            On.RoR2.EyeFlare.OnEnable += (orig, self) =>
            {
                if (self.directionSource && self.directionSource.parent && self.directionSource.parent.name == "AncientWispArmature")
                {
                    self.localScale = flareSize;
                }
                orig(self);
            };

            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupOrigin(SpawnCard spawncard)
        {
            if (allowOrigin)
            {
                Risky_Artifacts.Artifacts.Origin.AddSpawnCard(spawncard, Risky_Artifacts.Artifacts.Origin.BossTier.t3);
            }
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new AWContent());
        }

        private void FixHitbox(GameObject go)
        {
            Destroy(go.GetComponent<HurtBox>());

            Component[] goComponents = go.GetComponentsInChildren<Transform>();
            Transform goTransform = null;
            Transform headTransform = null;
            foreach (Transform t in goComponents)
            {
                if (t.name == "chest")
                {
                    goTransform = t;
                }
                else if (t.name == "Head")
                {
                    headTransform = t;
                }
            }

            ModelLocator goModelLocator = go.GetComponent<ModelLocator>();
            goModelLocator.modelTransform.gameObject.layer = LayerIndex.entityPrecise.intVal;
            goModelLocator.noCorpse = true;

            HurtBoxGroup goHurtBoxGroup = goModelLocator.modelTransform.gameObject.GetComponent<HurtBoxGroup>();

            goModelLocator.modelTransform.localScale *= 1.8f;

            #region chest
            goTransform.gameObject.layer = LayerIndex.entityPrecise.intVal;
            CapsuleCollider goCollider = goTransform.gameObject.AddComponent<CapsuleCollider>();

            HurtBox goHurtBox = goTransform.gameObject.AddComponent<HurtBox>();
            goHurtBox.isBullseye = true;
            goHurtBox.healthComponent = go.GetComponent<HealthComponent>();
            goHurtBox.damageModifier = HurtBox.DamageModifier.Normal;
            goHurtBox.hurtBoxGroup = goHurtBoxGroup;
            goHurtBox.indexInGroup = 0;
            #endregion

            #region head
            headTransform.gameObject.layer = LayerIndex.entityPrecise.intVal;
            BoxCollider headCollider = headTransform.gameObject.AddComponent<BoxCollider>();
            headCollider.center += 0.3f * Vector3.up;

            HurtBox headHurtbox = headTransform.gameObject.AddComponent<HurtBox>();
            headHurtbox.isBullseye = false;
            headHurtbox.healthComponent = go.GetComponent<HealthComponent>();
            headHurtbox.damageModifier = HurtBox.DamageModifier.Normal;
            headHurtbox.isSniperTarget = true;
            headHurtbox.hurtBoxGroup = goHurtBoxGroup;
            headHurtbox.indexInGroup = 1;
            #endregion

            HurtBox[] goHurtBoxArray = new HurtBox[]
            {
                goHurtBox,
                headHurtbox
            };

            goHurtBoxGroup.bullseyeCount = 1;
            goHurtBoxGroup.hurtBoxes = goHurtBoxArray;
            goHurtBoxGroup.mainHurtBox = goHurtBox;
        }

        private void AssignSkills(SkillLocator skills)
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
            barrageDef.dontAllowPastMaxStocks = true;
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
            rainDef.dontAllowPastMaxStocks = true;
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
            enrageDef.dontAllowPastMaxStocks = true;
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
            AWContent.entityStates.Add(typeof(Enrage));
            AWContent.skillDefs.Add(enrageDef);
            AWContent.skillFamilies.Add(utilitySkillFamily);
        }

        private void RegisterLanguageTokens()
        {
            LanguageAPI.Add("MOFFEIN_ANCIENTWISP_BODY_NAME", "Ancient Wisp");
            LanguageAPI.Add("MOFFEIN_ANCIENTWISP_BODY_SUBTITLE", "Banished and Chained");
            LanguageAPI.Add("MOFFEIN_ANCIENTWISP_BODY_LORE", "Could this be the architect of the smaller wisps? Perhaps this Ancient Wisp is the puppeteer behind the fiery masks harassing me thus far, or is he also a greater devilish device of something greater..?\n\nThe heat coming off the purple flames is extreme. Ferocious columns of lightning threaten to short-out my suit, or worse, turn me into a blistered husk. What fuels these violent, burning apparitions?");
            //LanguageAPI.Add("ARCHWISP_BODY_NAME", "Archaic Wisp");
        }

        private void SetBodyStats(CharacterBody ancientWispBody)
        {
            ancientWispBody.baseNameToken = "MOFFEIN_ANCIENTWISP_BODY_NAME";
            ancientWispBody.subtitleNameToken = "MOFFEIN_ANCIENTWISP_BODY_SUBTITLE";
            ancientWispBody.baseMaxHealth = 2800f;
            ancientWispBody.levelMaxHealth = ancientWispBody.baseMaxHealth * 0.3f;
            ancientWispBody.baseArmor = 20f;
            ancientWispBody.baseDamage = 20f;
            ancientWispBody.levelDamage = ancientWispBody.baseDamage * 0.2f;
            ancientWispBody.baseRegen = 0f;
            ancientWispBody.levelRegen = ancientWispBody.baseRegen * 0.2f;
            ancientWispBody.portraitIcon = AWContent.assets.LoadAsset<Texture>("aw_noflames.png");
            ancientWispBody._defaultCrosshairPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/crosshair/simpledotcrosshair");
            ancientWispBody.hideCrosshair = false;

            AWContent.entityStates.Add(typeof(EntityStates.MoffeinAncientWispSkills.SpawnState));
            SerializableEntityStateType sest = new SerializableEntityStateType(typeof(EntityStates.MoffeinAncientWispSkills.SpawnState));
            ancientWispBody.preferredInitialStateType = sest;
            EntityStateMachine esm = ancientWispBody.GetComponent<EntityStateMachine>();
            esm.initialStateType = sest;
        }

        public void DirectorActions(GameObject masterObject)
        {
            CharacterSpawnCard ancientWispCSC = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            ancientWispCSC.name = "cscAncientWisp";
            ancientWispCSC.prefab = masterObject;
            ancientWispCSC.sendOverNetwork = true;
            ancientWispCSC.hullSize = HullClassification.Golem;
            ancientWispCSC.nodeGraphType = MapNodeGroup.GraphType.Ground;
            ancientWispCSC.requiredFlags = NodeFlags.None;
            ancientWispCSC.forbiddenFlags = NodeFlags.NoCharacterSpawn;
            ancientWispCSC.directorCreditCost = 1000;
            ancientWispCSC.occupyPosition = false;
            ancientWispCSC.loadout = new SerializableLoadout();
            ancientWispCSC.noElites = false;
            ancientWispCSC.forbiddenAsBoss = false;

            DirectorCard directorCard = new DirectorCard
            {
                spawnCard = ancientWispCSC,
                selectionWeight = 1,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = "",
                forbiddenUnlockable = "",
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            ancientWispCard = new DirectorAPI.DirectorCardHolder
            {
                Card = directorCard,
                MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                InteractableCategory = DirectorAPI.InteractableCategory.None
            };

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RiskyArtifacts"))
            {
                SetupOrigin(ancientWispCSC);
            }

            DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
            {
                if (!list.Contains(ancientWispCard))
                {
                    int addAW = -1;
                    switch (stage.stage)
                    {
                        case DirectorAPI.Stage.TitanicPlains:
                            addAW = titanicPlains;
                            break;
                        case DirectorAPI.Stage.DistantRoost:
                            addAW = distantRoost;
                            break;
                        case DirectorAPI.Stage.WetlandAspect:
                            addAW = wetlands;
                            break;
                        case DirectorAPI.Stage.AbandonedAqueduct:
                            addAW = aqueduct;
                            break;
                        case DirectorAPI.Stage.RallypointDelta:
                            addAW = rallypoint;
                            break;
                        case DirectorAPI.Stage.ScorchedAcres:
                            addAW = scorched;
                            break;
                        case DirectorAPI.Stage.AbyssalDepths:
                            addAW = abyss;
                            break;
                        case DirectorAPI.Stage.SirensCall:
                            addAW = sirens;
                            break;
                        case DirectorAPI.Stage.SkyMeadow:
                            addAW = skymeadow;
                            break;
                        case DirectorAPI.Stage.VoidCell:
                            addAW = voidCell;
                            break;
                        case DirectorAPI.Stage.ArtifactReliquary:
                            addAW = artifact;
                            break;
                        case DirectorAPI.Stage.GildedCoast:
                            addAW = gilded;
                            break;
                        case DirectorAPI.Stage.SunderedGrove:
                            addAW = stadia;
                            break;
                        default:
                            break;
                    }

                    //Todo: Replace when R2API updates
                    if (addAW < 0)
                    {
                        switch (stage.CustomStageName)
                        {
                            //Simulacrum
                            case "itgolemplains":
                                addAW = titanicPlains >= 0 ? 0 : -1;
                                break;
                            case "itdampcave":
                                addAW = abyss >= 0 ? 0 : -1;
                                break;
                            case "itancientloft":
                                addAW = aphSanct >= 0 ? 0 : -1;
                                break;
                            case "itfrozenwall":
                                addAW = rallypoint >= 0 ? 0 : -1;
                                break;
                            case "itgoolake":
                                addAW = aqueduct >= 0 ? 0 : -1;
                                break;
                            case "itskymeadow":
                                addAW = skymeadow >= 0 ? 0 : -1;
                                break;

                            //DLC1
                            case "ancientloft":
                                addAW = aphSanct;
                                break;
                            case "sulfurpools":
                                addAW = sulfur;
                                break;
                            case "snowyforest":
                                addAW = snowyForest;
                                break;
                        }
                    }

                    if (addAW >= 0)
                    {
                        directorCard.minimumStageCompletions = addAW;
                        list.Add(ancientWispCard);
                    }

                }
            };
        }

        private static void BuildLightningProjectile()
        {
            GameObject proj = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/nullifierprebombprojectile"), "MoffeinAWLightning", true);
            GameObject projGhost = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectileghosts/nullifierprebombghost"), "MoffeinAWLightningGhost", false);
            AWContent.projectilePrefabs.Add(proj);
            proj.AddComponent<LightningVerticalHit>();

            Destroy(proj.GetComponent<TeamAreaIndicator>());

            ProjectileImpactExplosion pie = proj.GetComponent<ProjectileImpactExplosion>();
            pie.lifetime = EntityStates.MoffeinAncientWispSkills.ChannelRain.explosionDelay;
            pie.lifetimeExpiredSoundString = "";
            pie.lifetimeExpiredSound = null;
            pie.impactEffect = null;
            pie.childrenProjectilePrefab = null;
            pie.blastRadius = EntityStates.MoffeinAncientWispSkills.ChannelRain.radius;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            pie.bonusBlastForce = Vector3.up * 1000f;
            pie.blastProcCoefficient = 1f;
            pie.childrenCount = 0;

            ProjectileDamage pd = proj.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Generic;

            Destroy(proj.GetComponent<AkEvent>());
            Destroy(proj.GetComponent<AkGameObj>());

            EntityStates.MoffeinAncientWispSkills.ChannelRain.projectilePrefab = proj;

            ProjectileController pc = proj.GetComponent<ProjectileController>();
            pc.ghostPrefab = projGhost;
            projGhost.AddComponent<LightningVisual>();
            projGhost.transform.localScale = Vector3.zero;
            Destroy(projGhost.GetComponent<AkEvent>());
            Destroy(projGhost.GetComponent<AkGameObj>());
        }

        private void ModifyArchWispProjectile()
        {
            GameObject proj = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/ArchWispCannon"), "MoffeinAncientWispCannon", true);
            //GameObject projGround = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/ArchWispGroundCannon"), "MoffeinAncientWispGroundCannon", true);

            ProjectileSimple ps = proj.GetComponent<ProjectileSimple>();
            ps.lifetime = 7f;

            ProjectileImpactExplosion pie = proj.GetComponent<ProjectileImpactExplosion>();
            pie.lifetime = 7f;

            proj.GetComponent<Rigidbody>().useGravity = false;

            GameObject projGround = PrefabAPI.InstantiateClone(pie.childrenProjectilePrefab, "MoffeinAncientWispCannonGround", true);
            Destroy(projGround.GetComponent<ProjectileDamageTrail>());
            pie.childrenProjectilePrefab = projGround;

            FireBarrage.projectilePrefab = proj;
            //FireRHCannon.projectilePrefabNoFire = projNoFire;

            AWContent.projectilePrefabs.Add(proj);
            AWContent.projectilePrefabs.Add(projGround);
        }

        private void BuildAI(GameObject masterObject)
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

        private void DisplaySetup(GameObject go)
        {
            Transform headTransform = null;

            Transform[] ts = go.GetComponentsInChildren<Transform>();
            foreach (Transform t in ts)
            {
                if (t.name == "Head")
                {
                    headTransform = t;
                    break;
                }
            }

            ChildLocator cl = go.GetComponentInChildren<ChildLocator>();
            Array.Resize(ref cl.transformPairs, cl.transformPairs.Length + 1);
            cl.transformPairs[cl.transformPairs.Length - 1] = new ChildLocator.NameTransformPair
            {
                name = "Head",
                transform = headTransform
            };

            PopulateDisplays();

            ItemDisplayRuleSet idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            List<ItemDisplayRuleSet.KeyAssetRuleGroup> equipmentList = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>
            {
                new ItemDisplayRuleSet.KeyAssetRuleGroup
                {
                    keyAsset = RoR2Content.Equipment.AffixPoison,
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = LoadDisplay("DisplayEliteUrchinCrown"),
                            childName = "Head",
                            localPos = new Vector3(0f, 0.4f, 0f),
                            localAngles = new Vector3(255f, 0f, 0f),
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                    }
                },

                new ItemDisplayRuleSet.KeyAssetRuleGroup
                {
                    keyAsset = RoR2Content.Equipment.AffixHaunted,
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = LoadDisplay("DisplayEliteStealthCrown"),
                            childName = "Head",
                            localPos = new Vector3(0f, 0.5f, 0f),
                            localAngles = new Vector3(260f, 0f, 0f),
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                    }
                },

                new ItemDisplayRuleSet.KeyAssetRuleGroup
                {
                    keyAsset = RoR2Content.Equipment.AffixWhite,
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = LoadDisplay("DisplayEliteIceCrown"),
                            childName = "Head",
                            localPos = new Vector3(0f, 0.5f, 0f),
                            localAngles = new Vector3(260f, 0f, 0f),
                            localScale = 0.05f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                    }
                },

                new ItemDisplayRuleSet.KeyAssetRuleGroup
                {
                    keyAsset = RoR2Content.Equipment.AffixBlue,
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = LoadDisplay("DisplayEliteRhinoHorn"),
                            childName = "Head",
                            localPos = new Vector3(0f, 0.38f, 0.06f),
                            localAngles = new Vector3(-20f, 0f, 0f),
                            localScale = 0.26f * Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = LoadDisplay("DisplayEliteRhinoHorn"),
                            childName = "Head",
                            localPos = new Vector3(0f, 0.28f, 0.1f),
                            localAngles = new Vector3(-10f, 0f, 0f),
                            localScale = 0.36f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                    }
                },

                new ItemDisplayRuleSet.KeyAssetRuleGroup
                {
                    keyAsset = RoR2Content.Equipment.AffixRed,
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = LoadDisplay("DisplayEliteHorn"),
                            childName = "Head",
                            localPos = new Vector3(-0.15f, 0.28f, 0f),
                            localAngles = new Vector3(0f, 20f, 0f),
                            localScale = 0.18f * Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = LoadDisplay("DisplayEliteHorn"),
                            childName = "Head",
                            localPos = new Vector3(0.075f, 0.28f, -0.05f),
                            localAngles = new Vector3(0f, -20f, 0f),
                            localScale = 0.18f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                    }
                }
            };

            idrs.keyAssetRuleGroups = equipmentList.ToArray();
            CharacterModel characterModel = go.GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>();
            characterModel.itemDisplayRuleSet = idrs;
            characterModel.itemDisplayRuleSet.GenerateRuntimeValues();
            itemDisplayPrefabs.Clear();
        }

        internal void PopulateDisplays()
        {
            ItemDisplayRuleSet itemDisplayRuleSet = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemDisplayRuleSet.keyAssetRuleGroups;

            for (int i = 0; i < item.Length; i++)
            {
                ItemDisplayRule[] rules = item[i].displayRuleGroup.rules;

                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab = rules[j].followerPrefab;
                    if (followerPrefab)
                    {
                        string name = followerPrefab.name;
                        string key = (name != null) ? name.ToLower() : null;
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
        }

        public GameObject LoadDisplay(string name)
        {
            if (itemDisplayPrefabs.ContainsKey(name.ToLower()))
            {
                if (itemDisplayPrefabs[name.ToLower()]) return itemDisplayPrefabs[name.ToLower()];
            }
            return null;
        }
        private Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();
    }
}
