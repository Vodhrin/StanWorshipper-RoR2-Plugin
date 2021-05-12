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

            if (damageInfo == null) { orig(self, damageInfo); return; }

            CharacterBody characterBody = self.gameObject.GetComponent<CharacterBody>();
            CharacterBody attackerCharacterBody = damageInfo.attacker.GetComponent<CharacterBody>();

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
