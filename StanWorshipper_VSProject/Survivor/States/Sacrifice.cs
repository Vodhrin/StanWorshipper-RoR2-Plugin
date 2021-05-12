using System.Collections.Generic;
using EntityStates;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using StanWorshipper.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace StanWorshipper.Survivor.States
{
    public class Sacrifice : BaseSkillState
    {
        public static float radius = 30;
        public static float healFraction = 0.08f;
        public static int explosionCount = 7;
        public static float damageCoefficient = 1.75f;
        public static float procCoefficient = 0.2f;
        public static float force;
        public static float damageScaling;
        public static float buffDuration = 7f;
        public static float moveSpeedBoost = 0.4f;
        public float baseDuration = 7f;

        private float explosionTimer;
        private float explosionInterval;
        private int explosionIndex;
        private float duration;
        private List<CharacterBody> weakStans;
        private List<CharacterBody> strongStans;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / Mathf.Sqrt(base.attackSpeedStat);
            this.explosionInterval = this.duration / Sacrifice.explosionCount;
            this.explosionIndex = 0;

            weakStans = new List<CharacterBody>();
            strongStans = new List<CharacterBody>();
            foreach (TeamComponent i in TeamComponent.GetTeamMembers(this.GetTeam()))
            {
                if(i.gameObject && i.body.master.minionOwnership.ownerMaster == this.characterBody.master && i.body.master.bodyPrefab)
                {
                    if(i.body.master.bodyPrefab == Survivor.Summons.weakStanFragmentBody)
                    {
                        weakStans.Add(i.body);
                    }
                    else if (i.body.master.bodyPrefab == Survivor.Summons.strongStanFragmentBody)
                    {
                        strongStans.Add(i.body);
                    }
                }
            }

            this.ApplyNetworkedTimedBuff(base.gameObject, Survivor.Buffs.sacrificeBuff, 1, Sacrifice.buffDuration);

        }
        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.explosionTimer -= Time.fixedDeltaTime;
            if (this.explosionTimer <= 0f)
            {
                if (this.explosionIndex >= explosionCount)
                {
                    if (base.isAuthority)
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
                else
                {
                    this.explosionTimer += this.explosionInterval;

                    this.Explode(base.characterBody, 1f, BlastAttack.FalloffModel.Linear, Survivor.Effects.sacrificeBaseEffect, new EffectData
                    {
                        origin = base.transform.position,
                        color = Color.red,
                        scale = 0.1f
                    });
                    this.Heal(base.characterBody, 1f * Sacrifice.healFraction);

                    foreach(CharacterBody i in weakStans)
                    {
                        if (!i) continue;
                        this.Heal(i, 0.5f * Sacrifice.healFraction);
                    }
                    foreach(CharacterBody i in strongStans)
                    {
                        if (!i) continue;
                        this.Explode(i, 1.8f, BlastAttack.FalloffModel.Linear, Survivor.Effects.sacrificeStanEffect, new EffectData 
                        {
                            origin = i.transform.position,
                            color = Color.red,
                            scale = 5f
                        });
                    }

                    this.explosionIndex++;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private void Explode(CharacterBody originBody, float mult, BlastAttack.FalloffModel falloffModel, GameObject effect, EffectData effectData)
        {
            if (originBody) 
            { 
                EffectManager.SpawnEffect(effect, effectData, true);

                if (base.isAuthority)
                {
                    new BlastAttack
                    {
                        attacker = originBody.gameObject,
                        inflictor = originBody.gameObject,
                        teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
                        baseDamage = this.damageStat * Sacrifice.damageCoefficient * mult,
                        procCoefficient = Sacrifice.procCoefficient,
                        baseForce = Sacrifice.force,
                        position = originBody.transform.position,
                        radius = Sacrifice.radius * mult,
                        crit = Util.CheckRoll(this.critStat, base.characterBody.master),
                        falloffModel = falloffModel,
                        attackerFiltering = AttackerFiltering.NeverHit
                    }.Fire();
                }
            }
        }

        private void Heal(CharacterBody target,  float mult)
        {
            if(target && NetworkServer.active)
            {
                target.healthComponent.Heal(target.healthComponent.fullHealth * mult, default(ProcChainMask));
            }
            else if(target && !NetworkServer.active)
            {
                NetworkIdentity networkIdentity = target.GetComponent<NetworkIdentity>();
                if (networkIdentity)
                {
                    new NetMessages.HealFractionMessage(networkIdentity.netId, mult)
                        .Send(NetworkDestination.Server);
                }
            }
        }

        private void ApplyNetworkedTimedBuff(GameObject gameObject, BuffIndex buffIndex, int stacks, float duration)
        {
            CharacterBody characterBody = gameObject.GetComponent<CharacterBody>();

            if (characterBody)
            {
                if (NetworkServer.active)
                {
                    for (int i = 0; i < stacks; i++)
                    {
                        characterBody.AddTimedBuff(buffIndex, duration);
                    }
                }
                else
                {
                    NetworkIdentity networkIdentity = gameObject.GetComponent<NetworkIdentity>();
                    if (networkIdentity)
                    {
                        new NetMessages.TimedBuffMessage(networkIdentity.netId, buffIndex, stacks, duration)
                            .Send(NetworkDestination.Server);
                    }
                }
            }
        }
    }
}