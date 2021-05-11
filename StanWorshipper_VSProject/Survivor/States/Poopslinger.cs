using EntityStates;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Projectile;
using StanWorshipper.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace StanWorshipper.Survivor.States
{
    public class Poopslinger : BaseSkillState
    {
		public static GameObject muzzleFlashEffectPrefab;
		public static float damageCoefficient = 6.5f;
		public static float fireDuration = 1f;
		public static float baseDuration = 2.4f;
		public static float arcAngle = -2f;
		public static float recoilAmplitude = 0.5f;
		public static float spreadBloomValue = 0.1f;

		private Ray projectileRay;
		private Transform modelTransform;
		private float duration;

		public override void OnEnter()
        {
			base.OnEnter();
			this.duration = Poopslinger.baseDuration / this.attackSpeedStat;
			this.modelTransform = base.GetModelTransform();
			base.StartAimMode(2f, false);

			this.FireGrenade("Muzzle");
		}
        public override void OnExit()
        {
			base.OnExit();
        }

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

		private void FireGrenade(string targetMuzzle)
		{

			Poopslinger.muzzleFlashEffectPrefab = Resources.Load<GameObject>("Prebas/effects/muzzleflashes/MuzzleflashCroco");

			PlaySound(Assets.PoopslingerFireSound, base.gameObject, this.attackSpeedStat);
			base.PlayAnimation("Gesture, Override", "FireShotgun", "FireShotgun.playbackRate", this.duration * 1.1f);

			//Same code from Engineer's primary but with a different projectile and a max grenade count of 1. 
			this.projectileRay = base.GetAimRay();
			if (this.modelTransform)
			{
				ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(targetMuzzle);
					if (transform)
					{
						this.projectileRay.origin = transform.position;
					}
				}
			}
			base.AddRecoil(-1f * Poopslinger.recoilAmplitude, -2f * Poopslinger.recoilAmplitude, -1f * Poopslinger.recoilAmplitude, 1f * Poopslinger.recoilAmplitude);
			if (Poopslinger.muzzleFlashEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(Poopslinger.muzzleFlashEffectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				float x = UnityEngine.Random.Range(0f, base.characterBody.spreadBloomAngle);
				float z = UnityEngine.Random.Range(0f, 360f);
				Vector3 up = Vector3.up;
				Vector3 axis = Vector3.Cross(up, this.projectileRay.direction);
				Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
				float y = vector.y;
				vector.y = 0f;
				float angle = Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f;
				float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f + Poopslinger.arcAngle;
				Vector3 forward = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * this.projectileRay.direction);
				ProjectileManager.instance.FireProjectile(Survivor.Projectiles.poopslingerProjectile, this.projectileRay.origin + new Vector3(0, 0.6f, 0), Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * Poopslinger.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
			base.characterBody.AddSpreadBloom(Poopslinger.spreadBloomValue);
		}

		private void PlaySound(string soundName, GameObject gameObject, float rate = 1)
		{
			NetworkIdentity networkIdentity = gameObject.GetComponent<NetworkIdentity>();
			if (networkIdentity)
			{
				new NetMessages.SoundMessage(networkIdentity.netId, soundName)
					.Send(NetworkDestination.Clients);
			}
		}
	}
}