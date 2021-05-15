using System.IO;
using System.Reflection;
using R2API;
using UnityEngine;

namespace StanWorshipper.Core
{
    internal static class Assets
    {
        public static AssetBundle mainAssetBundle = null;

        public static Sprite icon1;
        public static Sprite icon2;
        public static Sprite icon3;
        public static Sprite icon4;
        public static Sprite icon5;

        public static Sprite stanFace;
        public static Sprite stanFace2;
        public static Sprite stanFace2Large;
        public static Sprite emptyIcon;

        public static Material weakStanFireMaterial;

        public const string PoopslingerFireSound = "Poopslinger_Fire";
        public const string PoopslingerHitSound = "Poopslinger_Hit";
        public const string StrongStanSpawnSound = "StrongStan_spawn";
        public const string WeakStanSpawnSound = "WeakStan_spawn";
        public const string StanLaughSound = "StanLaugh";
        public const string StanScreamSound = "StanScream";

        public static void Initialize()
        {

            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StanWorshipper.stanworshipperassetbundle"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }

            icon1 = mainAssetBundle.LoadAsset<Sprite>("Skill1Icon");
            icon2 = mainAssetBundle.LoadAsset<Sprite>("Skill2Icon");
            icon3 = mainAssetBundle.LoadAsset<Sprite>("Skill3Icon");
            icon4 = mainAssetBundle.LoadAsset<Sprite>("Skill4Icon");
            icon5 = mainAssetBundle.LoadAsset<Sprite>("PassiveIcon");

            stanFace = mainAssetBundle.LoadAsset<Sprite>("StanFace");
            stanFace2 = mainAssetBundle.LoadAsset<Sprite>("StanFace2");
            stanFace2Large = mainAssetBundle.LoadAsset<Sprite>("StanFace2Large");
            emptyIcon = mainAssetBundle.LoadAsset<Sprite>("EmptyIcon");

            weakStanFireMaterial = mainAssetBundle.LoadAsset<Material>("FireMat");

            //Loads the soundbank.
            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("StanWorshipper.StanWorshipperBank.bnk"))
            {
                var array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }
        }
    }
}
