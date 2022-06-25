using EntityStates.Engi.EngiWeapon;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.MoffeinAncientWispSkills
{
	public class Enrage : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = Enrage.baseDuration;
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				base.PlayCrossfade("Gesture", "Enrage", "Enrage.playbackRate", this.duration, 0.2f);
			}
			this.soundID = Util.PlayAttackSpeedSound(VagrantMonster.ChargeMegaNova.chargingSoundString, base.gameObject, this.attackSpeedStat);
			if (NetworkServer.active)
            {
				//base.characterBody.AddBuff(BuffIndex.ArmorBoost);
				base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
			}
		}

        public override void OnExit()
        {
			if (!stoppedSound)
            {
				AkSoundEngine.StopPlayingID(this.soundID);
			}
			if (NetworkServer.active)
			{
				/*if (base.characterBody.HasBuff(BuffIndex.ArmorBoost))
				{
					base.characterBody.RemoveBuff(BuffIndex.ArmorBoost);
				}*/
				if (base.characterBody.HasBuff(RoR2Content.Buffs.Slow50))
				{
					base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
				}
			}
			//Util.PlaySound("Play_MoffeinAW_death", base.gameObject);
			base.OnExit();
        }

        public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.modelAnimator && this.modelAnimator.GetFloat("Enrage.activate") > 0.5f && !this.hasCastBuff)
			{
				AkSoundEngine.StopPlayingID(this.soundID);
				this.hasCastBuff = true;
				stoppedSound = true;
				Util.PlaySound(VagrantMonster.FireMegaNova.novaSoundString, base.gameObject);
				Util.PlaySound("Play_MoffeinAW_lightning", base.gameObject);
				if (NetworkServer.active)
                {
					EffectData effectData = new EffectData();
					effectData.origin = base.transform.position;
					effectData.SetNetworkedObjectReference(base.gameObject);
					EffectManager.SpawnEffect(Enrage.enragePrefab, effectData, true);
					for (int i = 0; i < 2; i++)
					{
						SummonEnemy();
					}

					if (!base.characterBody.HasBuff(AncientWisp.AncientWispPlugin.enrageBuff))
					{
						base.characterBody.AddBuff(AncientWisp.AncientWispPlugin.enrageBuff);
					}
				}
			}
			if (base.fixedAge >= this.duration && base.isAuthority && this.hasCastBuff)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		private void SummonEnemy()
		{
			DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(archWispCard != null ? archWispCard : minionCard, new DirectorPlacementRule
			{
				placementMode = DirectorPlacementRule.PlacementMode.Approximate,
				minDistance = 3f,
				maxDistance = 20f,
				spawnOnTarget = base.transform
			}, RoR2Application.rng);
			directorSpawnRequest.summonerBodyObject = base.gameObject;
			DirectorSpawnRequest directorSpawnRequest2 = directorSpawnRequest;
			directorSpawnRequest2.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest2.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult spawnResult)
			{
				spawnResult.spawnedInstance.GetComponent<Inventory>().CopyEquipmentFrom(base.characterBody.inventory);
			}));
			DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
		}

		public static float baseDuration = 2.5f;
		public static GameObject enragePrefab = EntityStates.AncientWispMonster.Enrage.enragePrefab;
		private Animator modelAnimator;
		private float duration;
		private bool hasCastBuff;
		private uint soundID;
		private bool stoppedSound = false;
		public static BuffIndex enrageBuff;
		public static CharacterSpawnCard minionCard = LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscGreaterWisp");
		public static CharacterSpawnCard archWispCard = null;
	}
}
