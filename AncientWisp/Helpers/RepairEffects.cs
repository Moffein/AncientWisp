using AncientWisp.Helpers;
using EntityStates.MoffeinAncientWispSkills;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AncientWisp
{
    class RepairEffects : MonoBehaviour
    {
        public static void Repair()
        {
            RepairEnrageEffect();
        }
        private static void RepairEnrageEffect()
        {
            GameObject effect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/AncientWispEnrage");
            Transform effectTransform = effect.transform.Find("SwingTrail");
            var effectRenderer = effectTransform.GetComponent<Renderer>();
            if (effectRenderer)
            {
                effectRenderer.material = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun").transform.Find("Fire").GetComponent<Renderer>().material;
            }
            Enrage.enragePrefab = effect;
        }

        public static void ModifyLightning(GameObject lightning)
        {
            //lightning.AddComponent<LightningNoise>();

            Transform[] lt = lightning.GetComponentsInChildren<Transform>();
            Transform effectTransform = null;
            foreach (Transform t in lt)
            {
                if (t.name == "Ring, Center")
                {
                    effectTransform = t;
                    break;
                }
            }

            var effectRenderer = effectTransform.GetComponent<Renderer>();
            if (effectRenderer)
            {
                effectRenderer.material = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun").transform.Find("Fire").GetComponent<Renderer>().material;
            }
        }

        public static void BuildLightningPrefab()
        {
            if (!fixedLightning)
            {
                fixedLightning = true;
                LightningVisual.lightningDelayPrefab = PrefabAPI.InstantiateClone(EntityStates.AncientWispMonster.ChannelRain.delayPrefab, "MoffeinAWLightningVisual", false);
                LightningVisual.lightningDelayPrefab.AddComponent<LightningNoise>();
                RepairEffects.ModifyLightning(LightningVisual.lightningDelayPrefab);
                DestroyOnTimer timer = LightningVisual.lightningDelayPrefab.AddComponent<DestroyOnTimer>();
                timer.duration = EntityStates.MoffeinAncientWispSkills.ChannelRain.explosionDelay;

                DelayBlast db = LightningVisual.lightningDelayPrefab.GetComponent<DelayBlast>();
                LightningVisual.lightningExplosionEffect = db.explosionEffect;
                Destroy(LightningVisual.lightningDelayPrefab.GetComponent<DelayBlast>());
            }
        }

        public static bool fixedLightning = false;
    }
}
