using System;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using EntityStates;

namespace StanWorshipper.States.SummonStates
{
    class WeakStanMain : BaseState
    {

        public static float baseDuration = 1f;
        public static float radius = 5.2f;
        public static float damageCoefficient = 1.4f;
        public static float force = 0f;
        private float burnTimer;
        private float interval;
        private float soundTimer;
        private float soundLength = 6.424f;
        private CharacterBody ownerBody;

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

        public override void OnEnter()
        {
            base.OnEnter();
            this.burnTimer = WeakStanMain.baseDuration;
            this.soundTimer = this.soundLength;

            this.ownerBody = this.characterBody.master.minionOwnership.ownerMaster.GetBody();

            this.modelAnimator = base.GetModelAnimator();
            base.PlayAnimation("Body", "Idle");
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

            if (this.soundTimer <= 0f)
            {
                Util.PlaySound(Assets.StanScreamSound, this.gameObject);
                this.soundTimer = soundLength;
            }

            this.burnTimer -= Time.deltaTime;
            if (this.burnTimer <= 0 && base.isAuthority)
            {
                this.interval = WeakStanMain.baseDuration / this.attackSpeedStat;
                if (this.ownerBody)
                {
                    this.damageStat = this.ownerBody.damage;
                    this.attackSpeedStat = this.ownerBody.attackSpeed;
                    this.critStat = this.ownerBody.crit;
                }
                base.characterBody.master.GetComponent<BaseAI>().ForceAcquireNearestEnemyIfNoCurrentEnemy();
                Explode();
                this.burnTimer = this.interval;
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
                        this.modelAnimator.SetFloat(WeakStanMain.pivotPitchCycle, Mathf.Clamp01(Util.Remap(quaternion.x * Mathf.Sign(quaternion.w), -1f, 1f, 0f, 1f)), 1f, Time.fixedDeltaTime);
                    }
                    if (this.hasPivotYawLayer)
                    {
                        this.modelAnimator.SetFloat(WeakStanMain.pivotYawCycle, Mathf.Clamp01(Util.Remap(quaternion.y * Mathf.Sign(quaternion.w), -1f, 1f, 0f, 1f)), 1f, Time.fixedDeltaTime);
                    }
                    if (this.hasPivotRollLayer)
                    {
                        this.modelAnimator.SetFloat(WeakStanMain.pivotRollCycle, Mathf.Clamp01(Util.Remap(quaternion.z * Mathf.Sign(quaternion.w), -1f, 1f, 0f, 1f)), 1f, Time.fixedDeltaTime);
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
                            this.modelAnimator.SetFloat(WeakStanMain.flyRate, Vector3.Magnitude(base.rigidbodyMotor.rigid.velocity));
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

        private void Explode()
        {

            EffectManager.SpawnEffect(StanWorshipperPlugin.weakStanBurnEffect, new EffectData
            {
                origin = base.transform.position,
                scale = 2.5f
            }, true); ;

            if (base.isAuthority)
            {
                new BlastAttack
                {
                    attacker = base.gameObject,
                    inflictor = base.gameObject,
                    teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
                    baseDamage = this.damageStat * WeakStanMain.damageCoefficient,
                    baseForce = WeakStanMain.force,
                    position = base.transform.position,
                    radius = WeakStanMain.radius,
                    crit = Util.CheckRoll(this.critStat, base.characterBody.master),
                    falloffModel = BlastAttack.FalloffModel.None,
                    attackerFiltering = AttackerFiltering.NeverHit
                }.Fire();
            }
        }
    }
}
