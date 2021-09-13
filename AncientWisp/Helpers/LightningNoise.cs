using AncientWisp.Helpers;
using EntityStates.MoffeinAncientWispSkills;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace AncientWisp
{
    class LightningNoise : MonoBehaviour
    {
        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (!playedSound && timer >= ChannelRain.explosionDelay - 0.02f)
            {
                playedSound = true;
                Util.PlaySound("Play_item_use_lighningArm", this.gameObject);
                EffectManager.SpawnEffect(LightningVisual.lightningExplosionEffect, new EffectData
                {
                    origin = this.transform.position,
                    rotation = this.transform.rotation
                }, false);
            }
        }

        private float timer = 0f;
        private bool playedSound = false;
    }
}
