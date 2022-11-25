using System.Collections.Generic;
using RoR2;
using UnityEngine;
using R2API;
using RoR2.Navigation;
using UnityEngine.AddressableAssets;
using System.Runtime.CompilerServices;

namespace AncientWisp
{
    public class Director
    {
        public Director(GameObject masterObject)
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
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            AWContent.AncientWispCard = new DirectorAPI.DirectorCardHolder
            {
                Card = directorCard,
                MonsterCategory = DirectorAPI.MonsterCategory.Champions
            };

            DirectorCard directorCardLoop = new DirectorCard
            {
                spawnCard = ancientWispCSC,
                selectionWeight = 1,
                preventOverhead = false,
                minimumStageCompletions = 5,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            AWContent.AncientWispLoopCard = new DirectorAPI.DirectorCardHolder
            {
                Card = directorCardLoop,
                MonsterCategory = DirectorAPI.MonsterCategory.Champions
            };

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RiskyArtifacts"))
            {
                SetupOrigin(ancientWispCSC);
            }

            DirectorCardCategorySelection dissonanceSpawns = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/MixEnemy/dccsMixEnemy.asset").WaitForCompletion();
            int categoryIndex = FindCategoryIndexByName(dissonanceSpawns, "Champions");
            if (categoryIndex >= 0) dissonanceSpawns.AddCard(categoryIndex, directorCard);

            DirectorCardCategorySelection familySpawns = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/Common/dccsWispFamily.asset").WaitForCompletion();
            categoryIndex = FindCategoryIndexByName(familySpawns, "Champions");
            if (categoryIndex >= 0) familySpawns.AddCard(categoryIndex, directorCard);

            foreach (StageSpawnInfo ssi in AncientWispPlugin.StageList)
            {
                DirectorAPI.DirectorCardHolder toAdd = ssi.GetMinStages() == 0 ? AWContent.AncientWispCard : AWContent.AncientWispLoopCard;

                SceneDef sd = ScriptableObject.CreateInstance<SceneDef>();
                sd.baseSceneNameOverride = ssi.GetStageName();

                DirectorAPI.Helpers.AddNewMonsterToStage(toAdd, false, DirectorAPI.GetStageEnumFromSceneDef(sd), ssi.GetStageName());
            }

        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupOrigin(SpawnCard spawncard)
        {
            if (AncientWispPlugin.allowOrigin)
            {
                Risky_Artifacts.Artifacts.Origin.AddSpawnCard(spawncard, Risky_Artifacts.Artifacts.Origin.BossTier.t3);
            }
        }

        //Minibosses
        //Basic Monsters
        //Champions
        public static int FindCategoryIndexByName(DirectorCardCategorySelection dcs, string categoryName)
        {
            for (int i = 0; i < dcs.categories.Length; i++)
            {
                if (string.CompareOrdinal(dcs.categories[i].name, categoryName) == 0)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
