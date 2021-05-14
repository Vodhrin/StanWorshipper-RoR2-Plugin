using EntityStates;
using RoR2;
using RoR2.Projectile;
using StanWorshipper.Core;
using UnityEngine;

namespace StanWorshipper.Survivor.States.SummonStates
{
    class FirePoopBlast : BaseState
    {
		public static GameObject muzzleflashEffectPrefab;
		public int projectileCount = 5;
		public float totalYawSpread = 5f;
		public float baseDuration = 2f;
		public float baseFireDuration = 0.5f;
		public float damageCoefficient = 1.75f;
		public float projectileSpeed = 300f;
		public static float force = 20f;
		public static float selfForce = 0f;
		public static string attackString = Assets.PoopslingerFireSound;
		public static string muzzleString = "MainEyeMuzzle";
		private CharacterBody ownerBody;
		private float duration;
		private float fireDuration;
		private int projectilesFired;
		private bool projectileSpreadIsYaw;


		public override void OnEnter()
		{
			base.OnEnter();
			FirePoopBlast.muzzleflashEffectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/MuzzleflashCroco");

			this.ownerBody = this.characterBody.master.minionOwnership.ownerMaster.GetBody();
			if (this.ownerBody)
			{
				this.damageStat = this.ownerBody.damage;
				this.attackSpeedStat = this.ownerBody.attackSpeed;
				this.critStat = this.ownerBody.crit;
			}

			this.duration = this.baseDuration / this.attackSpeedStat;
			this.fireDuration = this.baseFireDuration / this.attackSpeedStat;

			if (base.isAuthority)
			{
				base.healthComponent.TakeDamageForce(base.GetAimRay().direction * FirePoopBlast.selfForce, false, false);
			}
			if (UnityEngine.Random.value <= 0.5f)
			{
				this.projectileSpreadIsYaw = true;
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
				int num = Mathf.FloorToInt(base.fixedAge / this.fireDuration * (float)this.projectileCount);
				if (this.projectilesFired <= num && this.projectilesFired < this.projectileCount)
				{
					if (FirePoopBlast.muzzleflashEffectPrefab)
					{
						EffectManager.SimpleMuzzleFlash(FirePoopBlast.muzzleflashEffectPrefab, base.gameObject, FirePoopBlast.muzzleString, false);
					}
					Util.PlayAttackSpeedSound(FirePoopBlast.attackString, base.gameObject, this.attackSpeedStat);
					Ray aimRay = base.GetAimRay();
					float speedOverride = this.projectileSpeed;
					int num2 = Mathf.FloorToInt((float)this.projectilesFired - (float)(this.projectileCount - 1) / 2f);
					float bonusYaw = 0f;
					float bonusPitch = 0f;
					if (this.projectileSpreadIsYaw)
					{
						bonusYaw = (float)num2 / (float)(this.projectileCount - 1) * this.totalYawSpread;
					}
					else
					{
						bonusPitch = (float)num2 / (float)(this.projectileCount - 1) * this.totalYawSpread;
					}
					Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw, bonusPitch);
					ProjectileManager.instance.FireProjectile(Survivor.Projectiles.poopslingerProjectile, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * this.damageCoefficient, FirePoopBlast.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, speedOverride);
					this.projectilesFired++;
				}
			}
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

	}
}
