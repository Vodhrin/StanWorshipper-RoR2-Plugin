using EntityStates;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using StanWorshipper.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace StanWorshipper.Survivor.States
{
    public class SummonMinor : BaseSkillState
    {
        public float baseDuration = 1f;
        public static int count = 3;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration;
            this.PlaySound(Assets.WeakStanSpawnSound, base.gameObject);

            for (int i = 0; i < SummonMinor.count; i++) 
            {
            Vector3 position = base.characterBody.corePosition + new Vector3(Random.Range(-4f, 4f), 5, Random.Range(-4f, 4f));

            this.SpawnStanMinion(base.gameObject, "WeakStanFragmentMaster", position);
            }
        }
        
        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
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

        private void SpawnStanMinion(GameObject gameoObject, string masterName, Vector3 position)
        {
            CharacterBody characterBody = gameObject.GetComponent<CharacterBody>();
            if (NetworkServer.active)
            {
                CharacterMaster characterMaster = new MasterSummon
                {
                    masterPrefab = MasterCatalog.FindMasterPrefab(masterName),
                    position = position,
                    rotation = characterBody.transform.rotation,
                    summonerBodyObject = gameObject,
                    ignoreTeamMemberLimit = true,
                    teamIndexOverride = new TeamIndex?(TeamIndex.Player)
                }.Perform();
                characterMaster.inventory.GiveItem(RoR2Content.Items.HealthDecay.itemIndex, 35);
            }
            else
            {
                NetworkIdentity networkIdentity = base.gameObject.GetComponent<NetworkIdentity>();
                if (networkIdentity)
                {
                    new NetMessages.SpawnStanMinonMessage(networkIdentity.netId, masterName, position)
                        .Send(NetworkDestination.Server);
                }
            }
        }
    }
}