using EntityStates.Engi.EngiWeapon;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AncientWisp.Helpers
{
    class DeathExplosions : MonoBehaviour
    {

        public void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch > DeathExplosions.timeBetweenExplosions)
            {
                stopwatch -= DeathExplosions.timeBetweenExplosions;
                if (NetworkServer.active)
                {
                    EffectManager.SpawnEffect(DeathExplosions.archWispExplosion, new EffectData
                    {
                        origin = this.transform.position + new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), UnityEngine.Random.Range(-4.5f, 4.5f), UnityEngine.Random.Range(-4.5f, 4.5f)),
                        scale = 1.7f
                    }, true);
                    Util.PlaySound("Play_MoffeinAW_deathexplosion", this.gameObject);
                }
            }
        }

        public void Awake()
        {
            stopwatch = 0f;
            if (NetworkServer.active)
            {
                EffectManager.SpawnEffect(DeathExplosions.archWispExplosion, new EffectData
                {
                    origin = this.transform.position,
                    scale = 1.7f
                }, true);
                EffectManager.SpawnEffect(DeathExplosions.archWispExplosion, new EffectData
                {
                    origin = this.transform.position + 1.5f * Vector3.up,
                    scale = 1.7f
                }, true);
                Util.PlaySound("Play_MoffeinAW_deathexplosion", this.gameObject);
            }
        }

        private float stopwatch;
        public static GameObject archWispExplosion = Resources.Load<GameObject>("prefabs/effects/archwispdeath");
        public static float timeBetweenExplosions = 0.4f;
    }
}
