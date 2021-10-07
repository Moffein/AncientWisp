using EntityStates;
using RoR2;
using RoR2.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.MoffeinAncientWispSkills
{
	public class ChargeRain : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeRain.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Body", "ChargeRain", "ChargeRain.playbackRate", this.duration);

			Util.PlayAttackSpeedSound("Play_greater_wisp_attack", base.gameObject, this.attackSpeedStat * (2f / ChargeRain.baseDuration));
			Util.PlaySound("Play_MoffeinAW_spawn", base.gameObject);

			if (NetworkServer.active)
            {
				base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
            }
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

        public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new ChannelRain());
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}
		public static float baseDuration = 2f;
		public static GameObject effectPrefab = EntityStates.AncientWispMonster.ChargeRain.effectPrefab;
		public static GameObject delayPrefab = EntityStates.AncientWispMonster.ChargeRain.delayPrefab;

		private float duration;
	}
}
