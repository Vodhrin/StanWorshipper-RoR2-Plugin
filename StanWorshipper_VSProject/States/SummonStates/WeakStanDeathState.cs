using System;
using RoR2;
using UnityEngine;
using EntityStates;

namespace StanWorshipper.States.SummonStates
{
	public class WeakStanDeathState : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			AkSoundEngine.StopAll(this.gameObject);
			EntityState.Destroy(base.gameObject);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}
	}
}
