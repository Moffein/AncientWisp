using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace EntityStates.MoffeinAncientWispSkills
{
	public class FireBarrage : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Ray aimRay = base.GetAimRay();

			if (AncientWisp.AncientWispPlugin.AccurateEnemiesLoaded && AncientWisp.AncientWispPlugin.AccurateEnemiesCompat)
            {
				aimRay = AccurateEnemiesAimray(aimRay);
            }

			string text = "MuzzleRight";
			this.duration = FireBarrage.baseDuration / this.attackSpeedStat;
			this.durationBetweenShots = FireBarrage.baseDurationBetweenShots / this.attackSpeedStat;
			if (FireBarrage.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireBarrage.effectPrefab, base.gameObject, text, false);
			}
			if (base.isAuthority && base.modelLocator && base.modelLocator.modelTransform)
			{
				bulletsToFire = Math.Min(FireBarrage.maxBullets, Mathf.CeilToInt(FireBarrage.bulletCount * this.attackSpeedStat));
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(text);
					if (transform)
					{
						Vector3 forward = aimRay.direction;
						//Vector3 forward = new Vector3(aimRay.direction.x, aimRay.direction.y - 15f, aimRay.direction.z);
						RaycastHit raycastHit;
						if (Physics.Raycast(aimRay, out raycastHit, (float)LayerIndex.world.mask))
						{
							forward = raycastHit.point - transform.position;
						}
						ProjectileManager.instance.FireProjectile(FireBarrage.projectilePrefab, this.bulletCountCurrent < bulletCount/2 ? transform.position : aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireBarrage.damageCoefficient, FireBarrage.force, base.RollCrit());
					}
				}
			}
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				if (this.bulletCountCurrent >= bulletsToFire && base.fixedAge >= this.duration)
				{
					this.outer.SetNextStateToMain();
					return;
				}
				if (this.bulletCountCurrent < bulletsToFire && base.fixedAge >= this.durationBetweenShots)
				{
					FireBarrage fireRHCannon = new FireBarrage();
					fireRHCannon.bulletCountCurrent = this.bulletCountCurrent + 1;
					fireRHCannon.bulletsToFire = this.bulletsToFire;
					this.outer.SetNextState(fireRHCannon);
					return;
				}
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		public static GameObject projectilePrefab;
		public static GameObject effectPrefab = EntityStates.AncientWispMonster.FireRHCannon.effectPrefab;
		public static float baseDuration = 2f;
		public static float baseDurationBetweenShots = 0.12f;
		public static float damageCoefficient = 2.1f;
		public static float force = 20f;

		public static int maxBullets = 16;
		public static int bulletCount = 6;
		private float duration;
		private float durationBetweenShots;
		public int bulletCountCurrent = 1;
		public int bulletsToFire = 0;

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private Ray AccurateEnemiesAimray(Ray aimRay)
        {
			if (base.characterBody && !base.characterBody.isPlayerControlled)
			{
				HurtBox targetHurtbox = AccurateEnemies.Util.GetMasterAITargetHurtbox(base.characterBody.master);
				Ray newAimRay = AccurateEnemies.Util.PredictAimrayPS(aimRay, base.GetTeam(), AccurateEnemies.AccurateEnemiesPlugin.basePredictionAngle, FireBarrage.projectilePrefab, targetHurtbox);
				return newAimRay;
			}
			return aimRay;
        }
	}
}
