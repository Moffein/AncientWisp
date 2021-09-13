using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EntityStates.MoffeinAncientWispSkills
{
	public class ChargeBarrage : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeBarrage.baseDuration / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			//base.PlayAnimation("Gesture", "ChargeRHCannon", "ChargeRHCannon.playbackRate", this.duration);
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				int layerIndex = this.modelAnimator.GetLayerIndex("Gesture");
				if (this.modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Throw1"))
				{
					base.PlayCrossfade("Gesture", "Throw2", "Throw.playbackRate", this.duration / 0.3f, 0.2f);
				}
				else
				{
					base.PlayCrossfade("Gesture", "Throw1", "Throw.playbackRate", this.duration / 0.3f, 0.2f);
				}
			}
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component && ChargeBarrage.effectPrefab)
				{
					Transform transform = component.FindChild("MuzzleRight");
					if (transform)
					{
						this.chargeEffectRight = UnityEngine.Object.Instantiate<GameObject>(ChargeBarrage.effectPrefab, transform.position, transform.rotation);
						this.chargeEffectRight.transform.parent = transform;
					}
				}
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration);
			}
			Util.PlayAttackSpeedSound("Play_greater_wisp_attack", base.gameObject, this.attackSpeedStat * (2f / ChargeBarrage.baseDuration));
		}

		public override void OnExit()
		{
			base.OnExit();
			EntityState.Destroy(this.chargeEffectLeft);
			EntityState.Destroy(this.chargeEffectRight);
		}

		public override void Update()
		{
			base.Update();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			if (!playedSwing2 && this.duration - base.fixedAge < 0.6f)
            {
				if (this.modelAnimator)
				{
					playedSwing2 = true;
					int layerIndex = this.modelAnimator.GetLayerIndex("Gesture");
					if (this.modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Throw1"))
					{
						base.PlayCrossfade("Gesture", "Throw2", "Throw.playbackRate", FireBarrage.baseDuration * 6f, 0.1f);
					}
					else
					{
						base.PlayCrossfade("Gesture", "Throw1", "Throw.playbackRate", FireBarrage.baseDuration * 6f, 0.1f);
					}
				}
			}

			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				FireBarrage nextState = new FireBarrage();
				this.outer.SetNextState(nextState);
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		public static float baseDuration = 2.5f;
		public static GameObject effectPrefab = EntityStates.AncientWispMonster.ChargeRHCannon.effectPrefab;
		private float duration;

		private GameObject chargeEffectLeft;
		private GameObject chargeEffectRight;
		private Animator modelAnimator;
		private bool playedSwing2 = false;
	}
}
