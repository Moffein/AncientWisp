using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AncientWisp
{
    public class ItemDisplays
    {
        public ItemDisplays()
        {
            DisplaySetup(AncientWispPlugin.AncientWispObject);
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
                                localPos = new Vector3(0.18f, 0.28f, 0f),
                                localAngles = new Vector3(0f, 0f, 0f),
                                localScale = 0.18f * Vector3.one,
                                limbMask = LimbFlags.None
                            },
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = LoadDisplay("DisplayEliteHorn"),
                                childName = "Head",
                                localPos = new Vector3(-0.18f, 0.28f, 0f),
                                localAngles = new Vector3(0f, 0f, 0f),
                                localScale = 0.18f * new Vector3(-1f, 1f, 1f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                },

                new ItemDisplayRuleSet.KeyAssetRuleGroup
                {
                    keyAsset = DLC1Content.Elites.Earth.eliteEquipmentDef,
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = LoadDisplay("DisplayEliteMendingAntlers"),
                                childName = "Head",
                                localPos = new Vector3(0f, 0.33f, 0f),
                                localAngles = Vector3.zero,
                                localScale = Vector3.one,
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                },

                new ItemDisplayRuleSet.KeyAssetRuleGroup
                {
                    keyAsset = DLC1Content.Elites.Void.eliteEquipmentDef,
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = LoadDisplay("DisplayAffixVoid"),
                                childName = "Head",
                                localPos = new Vector3(0f, 0.2f, 0.2f),
                                localAngles = new Vector3(90f, 0f, 0f),
                                localScale = 0.2f * Vector3.one,
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
                if (rules != null)
                {
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
