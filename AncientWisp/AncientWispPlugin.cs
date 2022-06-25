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
using System.Linq;
using UnityEngine.AddressableAssets;

namespace AncientWisp
{
    [BepInDependency("com.Moffein.RiskyArtifacts", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInDependency("com.Moffein.ArchaicWisp", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.FixDamageTrailNullref")]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Moffein.AncientWisp", "AncientWisp", "1.5.0")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(DirectorAPI), nameof(PrefabAPI), nameof(LanguageAPI), nameof(SoundAPI), nameof(RecalculateStatsAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]

    public class AncientWispPlugin : BaseUnityPlugin
    {
        public static List<StageSpawnInfo> StageList = new List<StageSpawnInfo>();

        public static bool allowOrigin = true;
        public static bool archWispCompat = true;

        public static BuffDef enrageBuff;

        public static BodyIndex bodyIndex;

        public static GameObject AncientWispObject;

        public void LateSetup()
        {
            new ItemDisplays();
        }

        private static Type GetTypeFromName(String name)
        {
            Type[] types = EntityStateCatalog.stateIndexToType;
            return Type.GetType(name);
        }
        public void Awake()
        {
            FixEntityStates.RunFix();
            float flareSize = base.Config.Bind<float>(new ConfigDefinition("General", "Eye Flare Size"), 0.5f, new ConfigDescription("How big the flare effect on the eye should be. 0 disables.")).Value;
            allowOrigin = base.Config.Bind<bool>(new ConfigDefinition("General", "RiskyArtifacts - Add to Artifact of Origination Spawnpool"), true, new ConfigDescription("Allows this boss to spawn when Origination from Risky Artifacts is enabled.")).Value;
           
            string stages = base.Config.Bind<string>(new ConfigDefinition("Spawns", "Stage List"), "dampcavesimple, rootjungle, skymeadow, sulfurpools - loop, itdampcave, itskymeadow, goldshores, artifactworld", new ConfigDescription("What stages the boss will show up on. Add a '- loop' after the stagename to make it only spawn after looping. List of stage names can be found at https://github.com/risk-of-thunder/R2Wiki/wiki/List-of-scene-names")).Value;

            //parse stage
            stages = new string(stages.ToCharArray().Where(c => !System.Char.IsWhiteSpace(c)).ToArray());
            string[] splitStages = stages.Split(',');
            foreach (string str in splitStages)
            {
                string[] current = str.Split('-');

                string name = current[0];
                int minStages = 0;
                if (current.Length > 1)
                {
                    minStages = 5;
                }

                StageList.Add(new StageSpawnInfo(name, minStages));
            }
            new Tokens();
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

            AncientWispObject = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/AncientWispBody"), "MoffeinAncientWispBody", true);
            GameObject ancientWispMaster = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/AncientWispMaster"), "MoffeinAncientWispMaster", true);
            CharacterBody ancientWispBody = AncientWispObject.GetComponent<CharacterBody>();
            SkillLocator ancientWispSkills = AncientWispObject.GetComponent<SkillLocator>();

            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                AncientWispPlugin.bodyIndex = BodyCatalog.FindBodyIndex("MoffeinAncientWispBody");
            };

            //Credits to TimeSweeper for this fix
            RoR2.RoR2Application.onLoad += LateSetup;

            AncientWispObject.GetComponent<CameraTargetParams>().cameraParams = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/ImpBossBody").GetComponent<CameraTargetParams>().cameraParams;

            ancientWispMaster.GetComponent<CharacterMaster>().bodyPrefab = AncientWispObject;

            GameObject GrovetenderPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Gravekeeper/GravekeeperBody.prefab").WaitForCompletion();
            DeathRewards deathReward = AncientWispObject.GetComponent<DeathRewards>();
            deathReward.bossDropTable = GrovetenderPrefab.GetComponent<DeathRewards>().bossDropTable;

            SfxLocator ancientWispSfx = AncientWispObject.AddComponent<SfxLocator>();
            ancientWispSfx.barkSound = "Play_magmaWorm_idle_VO";
            ancientWispSfx.deathSound = "Play_MoffeinAW_death";


            SetBodyStats(ancientWispBody);
            new Director(ancientWispMaster);
            ModifyArchWispProjectile();
            BuildLightningProjectile();
            FixHitbox(AncientWispObject);

            new CreateSkills(ancientWispSkills);
            new CreateAI(ancientWispMaster);


            AWContent.masterPrefabs.Add(ancientWispMaster);
            AWContent.bodyPrefabs.Add(AncientWispObject);

            Interactor interactor = AncientWispObject.GetComponent<Interactor>();
            interactor.maxInteractionDistance = 6f;

            enrageBuff = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/EnrageAncientWisp");
            enrageBuff.iconSprite = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/WarCryBuff").iconSprite;
            enrageBuff.buffColor = new Color(0.8039216f, 0.482352942f, 0.843137264f);   //Taken from BanditReloaded

            On.RoR2.CharacterBody.OnDeathStart += (orig, self) =>
            {
                orig(self);
                if (self.bodyIndex == AncientWispPlugin.bodyIndex)
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

            if (archWispCompat && BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.ArchaicWisp"))
            {
                GrabArchWispCard();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void GrabArchWispCard()
        {
            EntityStates.MoffeinAncientWispSkills.Enrage.archWispCard = (CharacterSpawnCard)ArchaicWisp.ArchaicWispContent.ArchaicWispCard.Card.spawnCard;
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
            pie.childrenProjectilePrefab = projGround;

            //Destroy(projGround.GetComponent<ProjectileDamageTrail>());
            ProjectileDamageTrail pdt = projGround.GetComponent<ProjectileDamageTrail>();
            pdt.damageToTrailDpsFactor *= 2f;

            FireBarrage.projectilePrefab = proj;
            //FireRHCannon.projectilePrefabNoFire = projNoFire;

            AWContent.projectilePrefabs.Add(proj);
            AWContent.projectilePrefabs.Add(projGround);
        }
    }
    public class StageSpawnInfo
    {
        private string stageName;
        private int minStages;

        public StageSpawnInfo(string stageName, int minStages)
        {
            this.stageName = stageName;
            this.minStages = minStages;
        }

        public string GetStageName() { return stageName; }
        public int GetMinStages() { return minStages; }
    }
}
