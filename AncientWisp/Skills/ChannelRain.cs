using AncientWisp;
using AncientWisp.Helpers;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.MoffeinAncientWispSkills
{
	public class ChannelRain : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChannelRain.baseDuration;
			this.durationBetweenCast = ChannelRain.baseDuration / Mathf.Min(ChannelRain.explosionCount * this.attackSpeedStat, ChannelRain.maxExplosions) ;
			base.PlayCrossfade("Body", "ChannelRain", 0.3f);
			Util.PlaySound("Play_titanboss_shift_charge", base.gameObject);

			if (NetworkServer.active)
			{
				base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
			}
		}

		private void PlaceRain()
		{
			if (!base.isAuthority)
            {
				return;
            }
			Vector3 vector = Vector3.zero;
			Ray aimRay = base.GetAimRay();
			aimRay.origin += UnityEngine.Random.insideUnitSphere * ChannelRain.randomRadius;
			RaycastHit raycastHit;
			if (Physics.Raycast(aimRay, out raycastHit, (float)LayerIndex.world.mask))
			{
				vector = raycastHit.point;
			}
			if (vector != Vector3.zero)
			{
				TeamIndex teamIndex = base.characterBody.GetComponent<TeamComponent>().teamIndex;
				TeamIndex enemyTeam;
				if (teamIndex != TeamIndex.Player)
				{
					if (teamIndex == TeamIndex.Monster)
					{
						enemyTeam = TeamIndex.Player;
					}
					else
					{
						enemyTeam = TeamIndex.Neutral;
					}
				}
				else
				{
					enemyTeam = TeamIndex.Monster;
				}
				Transform transform = this.FindTargetClosest(vector, enemyTeam);
				Vector3 a = vector;
				if (transform)
				{
					a = transform.transform.position;
				}
				a += UnityEngine.Random.insideUnitSphere * ChannelRain.randomRadius;
				if (Physics.Raycast(new Ray
				{
					origin = a + Vector3.up * ChannelRain.randomRadius,
					direction = Vector3.down
				}, out raycastHit, 500f, LayerIndex.world.mask))
				{
					Vector3 point = raycastHit.point;
					Quaternion rotation;
					Vector3 rot = new Vector3(90f, 0f, 0f);
					rotation = Quaternion.Euler(rot);
					FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
					fireProjectileInfo.projectilePrefab = ChannelRain.projectilePrefab;
					fireProjectileInfo.position = point;
					fireProjectileInfo.rotation = rotation;
					fireProjectileInfo.owner = base.gameObject;
					fireProjectileInfo.damage = this.damageStat * ChannelRain.damageCoefficient;
					fireProjectileInfo.force = 2000f;
					fireProjectileInfo.crit = base.characterBody.RollCrit();
					ProjectileManager.instance.FireProjectile(fireProjectileInfo);
				}
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.castTimer += Time.fixedDeltaTime;
			if (this.castTimer >= this.durationBetweenCast)
			{
				this.PlaceRain();
				this.castTimer -= this.durationBetweenCast;
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new EndRain());
			}
		}

		private Transform FindTargetClosest(Vector3 point, TeamIndex enemyTeam)
		{
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(enemyTeam);
			float num = 99999f;
			Transform result = null;
			for (int i = 0; i < teamMembers.Count; i++)
			{
				float num2 = Vector3.SqrMagnitude(teamMembers[i].transform.position - point);
				if (num2 < num)
				{
					num = num2;
					result = teamMembers[i].transform;
				}
			}
			return result;
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}

        public override void OnExit()
        {
			if (NetworkServer.active)
			{
				if (base.characterBody.HasBuff(RoR2Content.Buffs.Slow50))
				{
					base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
				}
			}
			base.OnExit();
        }

        private float castTimer;

		public static float baseDuration = 7f;
		public static float explosionDelay = 1.3f;
		public static int explosionCount = 40;

		public static int maxExplosions = 72;

		public static float damageCoefficient = 2.1f;
		public static float randomRadius = 16f;
		public static float radius = 6f;
		public static GameObject projectilePrefab;


		private float duration;
		private float durationBetweenCast;
		private float totalExplosions;
	}
}
