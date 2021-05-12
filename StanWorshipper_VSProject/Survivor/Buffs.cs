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
            BuffDef sacrificeBuffDef = ScriptableObject.CreateInstance<BuffDef>();
            {
                sacrificeBuffDef.name = "Sacrifice";
                sacrificeBuffDef.iconSprite = Resources.Load<Sprite>("textures/bufficons/texMovespeedBuffIcon");
                sacrificeBuffDef.buffColor = Color.yellow;
                sacrificeBuffDef.canStack = false;
            };
            CustomBuff sacrificeCustomBuff = new CustomBuff(sacrificeBuffDef);
            BuffAPI.Add(sacrificeCustomBuff);
        }

        public static void GetBuffIndices()
        {
            sacrificeBuff = BuffCatalog.FindBuffIndex("Sacrifice");
        }
    }
}
