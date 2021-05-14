using UnityEngine;
using RoR2;

namespace StanWorshipper.Core
{
    internal static class Hooks
    {
        public static void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self)
            {
                if (self.HasBuff(Survivor.Buffs.sacrificeBuff))
                {
                    self.moveSpeed += self.moveSpeed * Survivor.States.Sacrifice.moveSpeedBoost;
                }
            }
        }

        public static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            // Handles the Bound by Blood passive attack-transferring

            // Prevents a bunch of errors to do with fall damage, non-player damageable objects(floating rocks), etc...
            if (damageInfo == null) { orig(self, damageInfo); return; }
            else if (!self || !self.gameObject.GetComponent<CharacterBody>()) { orig(self, damageInfo); return; }
            else if (!damageInfo.attacker || !damageInfo.attacker.GetComponent<CharacterBody>()) { orig(self, damageInfo); return; }

            var characterBody = self.gameObject.GetComponent<CharacterBody>() as CharacterBody;
            var attackerCharacterBody = damageInfo.attacker.GetComponent<CharacterBody>() as CharacterBody;

            // Intended to prevent errors arising from jellyfish who have just exploded and no longer have a CharacterBody
            // Probably unnecessary but this implementation of this feature is very precarious imo
            if(characterBody == null || attackerCharacterBody == null){ orig(self, damageInfo); return; }

            // Checks if the attacker is a Stan fragment using its name token which is probably gay
            if (attackerCharacterBody.baseNameToken == "STRONGSTANFRAGMENT_NAME" || attackerCharacterBody.baseNameToken == "WEAKSTANFRAGMENT_NAME")
            {
                damageInfo.attacker = attackerCharacterBody.master.minionOwnership.ownerMaster.GetBody().gameObject;
            }
            orig(self, damageInfo);
        }

        public static void BuffCatalog_Init(On.RoR2.BuffCatalog.orig_Init orig)
        {
            orig();
            Survivor.Buffs.GetBuffIndices();
        }
    }
}
