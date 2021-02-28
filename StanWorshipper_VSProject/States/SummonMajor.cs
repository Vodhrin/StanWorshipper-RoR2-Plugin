using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using R2API.Networking;
using R2API.Networking.Interfaces;
using EntityStates;

namespace StanWorshipper.States
{
    public class SummonMajor : BaseSkillState
    {
        public float baseDuration = 1f;

        private float duration;
        private Ray aimRay;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration;

            Vector3 spawnPos = GetSpawnPos();
            this.SpawnStanMinion(base.gameObject, "StrongStanFragmentMaster", spawnPos);
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

        private Vector3 GetSpawnPos()
        {
            this.aimRay = base.GetAimRay();
            RaycastHit raycastHit;
            if (Util.CharacterRaycast(base.gameObject, this.aimRay, out raycastHit, float.PositiveInfinity, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
            {
                if (Vector3.Distance(raycastHit.point, base.transform.position) > 150)
                {
                    return base.transform.position + new Vector3(0f, 5f, 0f);
                }
                else
                {
                    return raycastHit.point;
                }
            }
            else
            {
                return base.transform.position + new Vector3(0f, 5f, 0f);
            }
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
                characterMaster.inventory.GiveItem(ItemIndex.HealthDecay, 35);

                this.PlaySound(Assets.StrongStanSpawnSound, characterMaster.GetBodyObject());
            }
            else
            {
                NetworkIdentity networkIdentity = base.gameObject.GetComponent<NetworkIdentity>();
                if (networkIdentity)
                {
                    new NetMessages.SpawnStanMinonMessage(networkIdentity.netId, masterName, position, true, Assets.StrongStanSpawnSound)
                        .Send(NetworkDestination.Server);
                }
            }
        }
    }
}