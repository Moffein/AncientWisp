using AncientWisp;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EntityStates.MoffeinAncientWispSkills
{
    public class SpawnState : BaseState
    {
		public override void OnEnter()
		{
			base.OnEnter();

			this.duration = SpawnState.baseDuration;
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				base.PlayCrossfade("Gesture", "Enrage", "Enrage.playbackRate", this.duration, 0.2f);
			}
			if (base.rigidbodyMotor)
			{
				base.rigidbodyMotor.moveVector = Vector3.zero;
			}
			this.hasCastBuff = false;
			Util.PlaySound("Play_MoffeinAW_lightning", base.gameObject);
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.rigidbodyMotor)
			{
				base.rigidbodyMotor.moveVector = Vector3.zero;
			}
			if (this.modelAnimator && this.modelAnimator.GetFloat("Enrage.activate") > 0.5f && !this.hasCastBuff)
			{
                //Util.PlaySound("Play_MoffeinAW_spawn", base.gameObject);
                EffectData effectData = new EffectData
                {
                    origin = base.transform.position
                };
                effectData.SetNetworkedObjectReference(base.gameObject);
				EffectManager.SpawnEffect(SpawnState.enragePrefab, effectData, false);
				this.hasCastBuff = true;
			}
			if (base.characterMotor)
			{
				base.characterMotor.velocity = Vector3.zero;
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		public static float baseDuration = 3f;
		public static GameObject enragePrefab = EntityStates.AncientWispMonster.Enrage.enragePrefab;
		private Animator modelAnimator;
		private float duration;
		private bool hasCastBuff;
    }
}
