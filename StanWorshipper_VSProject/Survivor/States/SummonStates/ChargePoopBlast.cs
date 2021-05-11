using EntityStates;
using RoR2;
using UnityEngine;

namespace StanWorshipper.Survivor.States.SummonStates
{

	class ChargePoopBlast : BaseState
    {
        public static float baseDuration = 1f;
        public static string attackString;
        private float duration;

		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargePoopBlast.baseDuration / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			Util.PlayAttackSpeedSound(ChargePoopBlast.attackString, base.gameObject, this.attackSpeedStat);
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
				this.outer.SetNextState(this.GetNextState());
				return;
			}
		}

		public virtual EntityState GetNextState()
		{
			return new FirePoopBlast();
		}
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

	}
}
