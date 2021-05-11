using EntityStates;
using RoR2;
using StanWorshipper.Core;
using UnityEngine;

namespace StanWorshipper.Survivor.States.SummonStates
{
	public class WeakStanSpawnState : GenericCharacterSpawnState
	{
		public static GameObject spawnEffectPrefab;
		private CharacterBody ownerBody;

		public override void OnEnter()
		{
			base.OnEnter();
			spawnEffectPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFXArchWisp");
			Util.PlaySound(Assets.StanScreamSound, this.gameObject);
			this.ownerBody = this.characterBody.master.minionOwnership.ownerMaster.GetBody();

			if (this.ownerBody)
			{
				float ownerCurrentBaseHealth = this.ownerBody.baseMaxHealth + (this.ownerBody.levelMaxHealth * (this.ownerBody.level - 1));
				this.healthComponent.health += ownerBody.maxHealth + ownerBody.maxShield - ownerCurrentBaseHealth;
				var armor = this.characterBody.armor;
				armor = ownerBody.armor;

				Vector3 forward = this.ownerBody.inputBank.aimDirection;
				this.rigidbody.velocity = forward * 40f;
			}

			if (WeakStanSpawnState.spawnEffectPrefab)
			{
				EffectManager.SpawnEffect(spawnEffectPrefab, new EffectData
				{
					origin = base.characterBody.corePosition,
					scale = 3f
				}, true);
			}
			this.outer.SetNextStateToMain();
		}
	}
}
