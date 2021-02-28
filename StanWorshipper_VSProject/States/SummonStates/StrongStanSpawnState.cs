using System;
using RoR2;
using UnityEngine;
using EntityStates;

namespace StanWorshipper.States.SummonStates
{
	public class StrongStanSpawnState : GenericCharacterSpawnState
	{
		public static GameObject spawnEffectPrefab;
		private CharacterBody ownerBody;

		public override void OnEnter()
		{
			base.OnEnter();
			spawnEffectPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFXArchWisp");
			Util.PlaySound(Assets.StanLaughSound, this.gameObject);
			this.ownerBody = this.characterBody.master.minionOwnership.ownerMaster.GetBody();

            if (this.ownerBody)
            {
				float ownerCurrentBaseHealth = this.ownerBody.baseMaxHealth + (this.ownerBody.levelMaxHealth * (this.ownerBody.level - 1));
				this.healthComponent.health += ownerBody.maxHealth + ownerBody.maxShield - ownerCurrentBaseHealth;
				var armor = this.characterBody.armor;
				armor = ownerBody.armor;
            }

			if (StrongStanSpawnState.spawnEffectPrefab)
			{
				EffectManager.SpawnEffect(spawnEffectPrefab, new EffectData
				{
					origin = base.characterBody.corePosition,
					scale = 16f
				}, true);
			}
			this.outer.SetNextStateToMain();
		}
	}
}
