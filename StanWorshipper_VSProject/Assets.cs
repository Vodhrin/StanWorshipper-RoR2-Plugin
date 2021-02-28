using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using R2API;

namespace StanWorshipper
{
    public static class Assets
    {

        public static AssetBundle MainAssetBundle = null;
        public static AssetBundleResourcesProvider Provider;

        public static Sprite icon1;
        public static Sprite icon2;
        public static Sprite icon3;
        public static Sprite icon4;

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

        public static void InitializeAssets()
        {

            if (MainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StanWorshipper.stanworshipperassetbundle"))
                {
                    MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                    Provider = new AssetBundleResourcesProvider("@StanWorshipper", MainAssetBundle);
                }
            }

            icon1 = MainAssetBundle.LoadAsset<Sprite>("Skill1Icon");
            icon2 = MainAssetBundle.LoadAsset<Sprite>("Skill2Icon");
            icon3 = MainAssetBundle.LoadAsset<Sprite>("Skill3Icon");
            icon4 = MainAssetBundle.LoadAsset<Sprite>("Skill4Icon");

            stanFace = MainAssetBundle.LoadAsset<Sprite>("StanFace");
            stanFace2 = MainAssetBundle.LoadAsset<Sprite>("StanFace2");
            stanFace2Large = MainAssetBundle.LoadAsset<Sprite>("StanFace2Large");
            emptyIcon = MainAssetBundle.LoadAsset<Sprite>("EmptyIcon");

            weakStanFireMaterial = MainAssetBundle.LoadAsset<Material>("FireMat");

            //Loads the soundbank.
            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("StanWorshipper.StanWorshipperBank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }
        }
    }
}
