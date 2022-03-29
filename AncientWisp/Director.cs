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
                MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                InteractableCategory = DirectorAPI.InteractableCategory.None
            };

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RiskyArtifacts"))
            {
                SetupOrigin(ancientWispCSC);
            }

            DirectorCardCategorySelection dissonanceSpawns = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/MixEnemy/dccsMixEnemy.asset").WaitForCompletion();
            dissonanceSpawns.AddCard(0, directorCard);  //0 is Champions

            /*
            Debug.Log("\n\n\n\n\n\nDissonance Cards:");
            foreach (DirectorCard dc in dissonanceSpawns.categories[0].cards)
            {
                Debug.Log(dc.spawnCard.name);
            }
            */

            DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
            {
                if (!list.Contains(AWContent.AncientWispCard))
                {
                    bool shouldSpawn = false;
                    int minStages = 0;
                    foreach (StageSpawnInfo ssi in AncientWispPlugin.StageList)
                    {
                        if (ssi.GetStageName() == stage.CustomStageName)
                        {
                            shouldSpawn = true;
                            minStages = ssi.GetMinStages();
                            break;
                        }
                    }

                    if (shouldSpawn && Run.instance.stageClearCount >= minStages)   //This skips having to make a card for each unique minStageCompletions
                    {
                        //directorCard.minimumStageCompletions = minStages; //Don't modify the static variable
                        list.Add(AWContent.AncientWispCard);
                    }
                }
            };
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupOrigin(SpawnCard spawncard)
        {
            if (AncientWispPlugin.allowOrigin)
            {
                Risky_Artifacts.Artifacts.Origin.AddSpawnCard(spawncard, Risky_Artifacts.Artifacts.Origin.BossTier.t3);
            }
        }
    }
}
