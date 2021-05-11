using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace StanWorshipper.Core
{
   internal class NetMessages
    {
        public static void Initialize()
        {
            NetworkingAPI.RegisterMessageType<NetMessages.SoundMessage>();
            NetworkingAPI.RegisterMessageType<NetMessages.SpawnStanMinonMessage>();
            NetworkingAPI.RegisterMessageType<NetMessages.HealFractionMessage>();
            NetworkingAPI.RegisterMessageType<NetMessages.TimedBuffMessage>();
        }

        public class SoundMessage : INetMessage
        {
            private NetworkInstanceId targetId;
            private string soundName;
            private float rate;

            public SoundMessage()
            {
            }

            public SoundMessage(NetworkInstanceId targetId, string soundName, float rate = 1f)
            {
                this.targetId = targetId;
                this.soundName = soundName;
                this.rate = rate;
            }

            public void Serialize(NetworkWriter writer)
            {
                writer.Write(this.targetId);
                writer.Write(this.soundName);
                writer.Write(this.rate);
            }

            public void Deserialize(NetworkReader reader)
            {
                this.targetId = reader.ReadNetworkId();
                this.soundName = reader.ReadString();
                this.rate = reader.ReadSingle();
            }

            public void OnReceived()
            {
                GameObject gameObject = Util.FindNetworkObject(this.targetId);
                if (!gameObject)
                {
                    StanWorshipperPlugin.logger.LogMessage("Error syncing sounds: target gameobject does not exist!");
                    return;
                }

                Util.PlayAttackSpeedSound(soundName, gameObject, rate);
            }
        }

        public class SpawnStanMinonMessage : INetMessage
        {
            private NetworkInstanceId targetId;
            private string masterName;
            private Vector3 position;
            private bool playSound;
            private string soundName;

            public SpawnStanMinonMessage()
            {
            }

            public SpawnStanMinonMessage(NetworkInstanceId targetId, string masterName, Vector3 position, bool playSound = false, string soundName = "")
            {
                this.targetId = targetId;
                this.masterName = masterName;
                this.position = position;
                this.playSound = playSound;
                this.soundName = soundName;
            }

            public void Serialize(NetworkWriter writer)
            {
                writer.Write(targetId);
                writer.Write(masterName);
                writer.Write(position);
                writer.Write(playSound);
                writer.Write(soundName);
            }

            public void Deserialize(NetworkReader reader)
            {
                this.targetId = reader.ReadNetworkId();
                this.masterName = reader.ReadString();
                this.position = reader.ReadVector3();
                this.playSound = reader.ReadBoolean();
                this.soundName = reader.ReadString();
            }

            public void OnReceived()
            {
                GameObject gameObject = Util.FindNetworkObject(this.targetId);
                CharacterBody characterBody = gameObject.GetComponent<CharacterBody>();

                if (!gameObject) 
                {
                    StanWorshipperPlugin.logger.LogMessage("Error spawning stan fragment on client: target id was not found!");
                    return;
                }

                CharacterMaster characterMaster = new MasterSummon
                {
                    masterPrefab = MasterCatalog.FindMasterPrefab(masterName),
                    position = position,
                    rotation = characterBody.transform.rotation,
                    summonerBodyObject = gameObject,
                    ignoreTeamMemberLimit = true,
                    teamIndexOverride = new TeamIndex?(TeamIndex.Player)
                }.Perform();
                characterMaster.inventory.GiveItem(RoR2Content.Items.HealthDecay.itemIndex, 30);

                if (this.playSound & this.soundName != "")
                {
                    new NetMessages.SoundMessage(characterMaster.GetBodyObject().GetComponent<NetworkIdentity>().netId, soundName)
                        .Send(NetworkDestination.Clients);
                }
            }
        }

        public class HealFractionMessage : INetMessage
        {
            private NetworkInstanceId targetId;
            private float mult;

            public HealFractionMessage()
            {
            }

            public HealFractionMessage(NetworkInstanceId targetId, float mult)
            {
                this.targetId = targetId;
                this.mult = mult;
            }
            public void Serialize(NetworkWriter writer)
            {
                writer.Write(targetId);
                writer.Write(mult);
            }

            public void Deserialize(NetworkReader reader)
            {
                this.targetId = reader.ReadNetworkId();
                this.mult = reader.ReadSingle();
            }

            public void OnReceived()
            {
                GameObject gameObject = Util.FindNetworkObject(this.targetId);
                HealthComponent healthComponent = gameObject.GetComponent<HealthComponent>();

                if (!gameObject)
                {
                    StanWorshipperPlugin.logger.LogMessage("Error syncing heal: target id not found!");
                    return;
                }

                healthComponent.Heal(healthComponent.fullHealth * mult, default(ProcChainMask));
            }
        }

        public class TimedBuffMessage : INetMessage
        {
            private NetworkInstanceId targetId;
            private BuffIndex buffIndex;
            private int stacks;
            private float duration;

            public TimedBuffMessage()
            {
            }

            public TimedBuffMessage(NetworkInstanceId targetId, BuffIndex buffIndex, int stacks, float duration)
            {
                this.targetId = targetId;
                this.buffIndex = buffIndex;
                this.stacks = stacks;
                this.duration = duration;
            }

            public void Serialize(NetworkWriter writer)
            {
                writer.Write(this.targetId);
                writer.WriteBuffIndex(this.buffIndex);
                writer.Write(this.stacks);
                writer.Write(this.duration);
            }

            public void Deserialize(NetworkReader reader)
            {
                this.targetId = reader.ReadNetworkId();
                this.buffIndex = reader.ReadBuffIndex();
                this.stacks = reader.ReadInt32();
                this.duration = reader.ReadSingle();
            }

            public void OnReceived()
            {
                CharacterBody characterBody = Util.FindNetworkObject(this.targetId).GetComponent<CharacterBody>();

                if (!characterBody)
                {
                    StanWorshipperPlugin.logger.LogMessage("Error applying timed buff: character body does not exist!");
                    return;
                }

                for (int i = 0; i < stacks; i++)
                {
                    characterBody.AddTimedBuff(buffIndex, duration);
                }
            }
        }
    }
}
