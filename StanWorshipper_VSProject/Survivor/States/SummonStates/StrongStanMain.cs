using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using StanWorshipper.Core;
using UnityEngine;

namespace StanWorshipper.Survivor.States.SummonStates
{
    class StrongStanMain : BaseState
    {
        public static bool isSacrificeActive;

        private Animator modelAnimator;
        private bool skill1InputReceived;
        private bool skill2InputReceived;
        private bool skill3InputReceived;
        private bool skill4InputReceived;
        private bool hasPivotPitchLayer;
        private bool hasPivotYawLayer;
        private bool hasPivotRollLayer;
        private static readonly int pivotPitchCycle = Animator.StringToHash("pivotPitchCycle");
        private static readonly int pivotYawCycle = Animator.StringToHash("pivotYawCycle");
        private static readonly int pivotRollCycle = Animator.StringToHash("pivotRollCycle");
        private static readonly int flyRate = Animator.StringToHash("fly.rate");

        private float aiTimer;
        private float soundTimer;
        private float soundLength = 16.301f;
        private CharacterBody ownerBody;

        public override void OnEnter()
        {
            base.OnEnter();
            this.modelAnimator = base.GetModelAnimator();
            base.PlayAnimation("Body", "Idle");
            this.soundTimer = this.soundLength;
            if (this.modelAnimator)
            {
                this.hasPivotPitchLayer = (this.modelAnimator.GetLayerIndex("PivotPitch") != -1);
                this.hasPivotYawLayer = (this.modelAnimator.GetLayerIndex("PivotYaw") != -1);
                this.hasPivotRollLayer = (this.modelAnimator.GetLayerIndex("PivotRoll") != -1);
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.soundTimer -= Time.deltaTime;
            this.aiTimer -= Time.deltaTime;

            if (this.soundTimer <= 0f)
            {
                Util.PlaySound(Assets.StanLaughSound, this.gameObject);
                this.soundTimer = soundLength;
            }

            if (this.aiTimer <= 0 && base.isAuthority)
            {
                base.characterBody.master.GetComponent<BaseAI>().ForceAcquireNearestEnemyIfNoCurrentEnemy();
                this.aiTimer = 1f;
            }

            if (base.rigidbodyDirection)
            {
                Quaternion rotation = base.transform.rotation;
                Quaternion rhs = Util.QuaternionSafeLookRotation(base.rigidbodyDirection.aimDirection);
                Quaternion quaternion = Quaternion.Inverse(rotation) * rhs;
                if (this.modelAnimator)
                {
                    if (this.hasPivotPitchLayer)
                    {
                        this.modelAnimator.SetFloat(StrongStanMain.pivotPitchCycle, Mathf.Clamp01(Util.Remap(quaternion.x * Mathf.Sign(quaternion.w), -1f, 1f, 0f, 1f)), 1f, Time.fixedDeltaTime);
                    }
                    if (this.hasPivotYawLayer)
                    {
                        this.modelAnimator.SetFloat(StrongStanMain.pivotYawCycle, Mathf.Clamp01(Util.Remap(quaternion.y * Mathf.Sign(quaternion.w), -1f, 1f, 0f, 1f)), 1f, Time.fixedDeltaTime);
                    }
                    if (this.hasPivotRollLayer)
                    {
                        this.modelAnimator.SetFloat(StrongStanMain.pivotRollCycle, Mathf.Clamp01(Util.Remap(quaternion.z * Mathf.Sign(quaternion.w), -1f, 1f, 0f, 1f)), 1f, Time.fixedDeltaTime);
                    }
                }
            }
            if (base.isAuthority)
            {
                if (base.inputBank)
                {
                    if (base.rigidbodyMotor)
                    {
                        base.rigidbodyMotor.moveVector = base.inputBank.moveVector * base.characterBody.moveSpeed;
                        if (this.modelAnimator)
                        {
                            this.modelAnimator.SetFloat(StrongStanMain.flyRate, Vector3.Magnitude(base.rigidbodyMotor.rigid.velocity));
                        }
                    }
                    if (base.rigidbodyDirection)
                    {
                        base.rigidbodyDirection.aimDirection = base.GetAimRay().direction;
                    }
                    this.skill1InputReceived = base.inputBank.skill1.down;
                    this.skill2InputReceived = base.inputBank.skill2.down;
                    this.skill3InputReceived = base.inputBank.skill3.down;
                    this.skill4InputReceived = base.inputBank.skill4.down;
                }
                if (base.skillLocator)
                {
                    if (this.skill1InputReceived && base.skillLocator.primary)
                    {
                        base.skillLocator.primary.ExecuteIfReady();
                    }
                    if (this.skill2InputReceived && base.skillLocator.secondary)
                    {
                        base.skillLocator.secondary.ExecuteIfReady();
                    }
                    if (this.skill3InputReceived && base.skillLocator.utility)
                    {
                        base.skillLocator.utility.ExecuteIfReady();
                    }
                    if (this.skill4InputReceived && base.skillLocator.special)
                    {
                        base.skillLocator.special.ExecuteIfReady();
                    }
                }
            }
        }
    }
}
