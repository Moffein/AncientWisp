using EntityStates.MoffeinAncientWispSkills;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AncientWisp.Helpers
{
    public class LightningVerticalHit : MonoBehaviour
    {
        public void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (!fired && NetworkServer.active && stopwatch > ChannelRain.explosionDelay - 0.02f)
            {
                fired = true;
                ProjectileController pc = this.gameObject.GetComponent<ProjectileController>();
                ProjectileDamage pd = this.gameObject.GetComponent<ProjectileDamage>();
                if (pc && pd)
                {
                    BulletAttack ba = new BulletAttack
                    {
                        owner = pc.owner,
                        weapon = this.gameObject,
                        origin = this.transform.position,
                        aimVector = Vector3.up,
                        minSpread = 0f,
                        maxSpread = 0f,
                        bulletCount = 1u,
                        procCoefficient = 0.5f,
                        damage = pd.damage * 0.5f,
                        force = pd.force * 0.4f,
                        falloffModel = BulletAttack.FalloffModel.None,
                        tracerEffectPrefab = null,
                        muzzleName = "",
                        hitEffectPrefab = null,
                        isCrit = pd.crit,
                        HitEffectNormal = false,
                        radius = 3f,
                        smartCollision = true,
                        maxDistance = 16f,
                        stopperMask = LayerIndex.world.mask
                    };
                    ba.damageType |= DamageType.AOE;
                    ba.Fire();
                }
            }
        }

        private bool fired = false;
        private float stopwatch = 0f;
        //public static GameObject tracer = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/tracers/tracertoolbotrebar");
    }
}
