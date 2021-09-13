using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EntityStates.MoffeinAncientWispSkills
{
	public class EndRain : BaseState
	{
		// Token: 0x0600355B RID: 13659 RVA: 0x000E0A70 File Offset: 0x000DEC70
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = EndRain.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Body", "EndRain", "EndRain.playbackRate", this.duration);
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void Update()
		{
			base.Update();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		public static float baseDuration = 3f;
		public static GameObject effectPrefab = EntityStates.AncientWispMonster.EndRain.effectPrefab;
		public static GameObject delayPrefab = EntityStates.AncientWispMonster.EndRain.delayPrefab;

		private float duration;
	}
}
