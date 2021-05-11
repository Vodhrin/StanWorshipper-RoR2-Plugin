using UnityEngine;
using RoR2;
using R2API;

namespace StanWorshipper.Survivor
{
    internal static class Buffs
    {
        public static BuffIndex sacrificeBuff;

        public static void Initialize()
        {
            BuffDef sacrificeBuffDef = new BuffDef
            {
                name = "Sacrifice",
                iconSprite = Resources.Load<Sprite>("textures/bufficons/texMovespeedBuffIcon"),
                buffColor = Color.yellow,
                canStack = false
            };
            CustomBuff sacrificeCustomBuff = new CustomBuff(sacrificeBuffDef);
            sacrificeBuff = BuffAPI.Add(sacrificeCustomBuff);
            StanWorshipperPlugin.buffDefs.Add(sacrificeBuffDef);
        }
    }
}
