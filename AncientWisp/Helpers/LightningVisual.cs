using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using EntityStates.MoffeinAncientWispSkills;

namespace AncientWisp.Helpers
{
    class LightningVisual : MonoBehaviour
    {
        public void Awake()
        {
			RepairEffects.BuildLightningPrefab();
			lightning = UnityEngine.Object.Instantiate<GameObject>(LightningVisual.lightningDelayPrefab, this.transform.position, this.transform.rotation);
			lightning.transform.localScale = new Vector3(ChannelRain.radius, ChannelRain.radius, 1f);
			ScaleParticleSystemDuration component2 = lightning.GetComponent<ScaleParticleSystemDuration>();
			if (component2)
			{
				component2.newDuration = ChannelRain.explosionDelay;
			}
		}

		public void OnDestroy()
        {
			if (lightning)
            {
				Destroy(lightning);
            }
        }

		private GameObject lightning;
        public static GameObject lightningDelayPrefab;
		public static GameObject lightningExplosionEffect;
    }
}
