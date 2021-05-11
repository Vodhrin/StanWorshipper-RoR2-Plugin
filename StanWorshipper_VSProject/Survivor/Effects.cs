using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using R2API;

using StanWorshipper.Core;
namespace StanWorshipper.Survivor
{
    internal static class Effects
    {
        public static GameObject poopslingerExplosionEffect;
        public static GameObject sacrificeBaseEffect;
        public static GameObject sacrificeStanEffect;
        public static GameObject weakStanBurnEffect;

        public static void Initialize()
        {
            poopslingerExplosionEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/effects/impacteffects/BeetleSpitExplosion"), "PoopslingerExplosionEffect", true);
            if (!poopslingerExplosionEffect.GetComponent<NetworkIdentity>()) poopslingerExplosionEffect.AddComponent<NetworkIdentity>();
            poopslingerExplosionEffect.GetComponent<EffectComponent>().soundName = Assets.PoopslingerHitSound;
            EffectAPI.AddEffect(poopslingerExplosionEffect);

            sacrificeBaseEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/effects/ArtifactShellExplosion"), "SacrificeBaseEffect", true);
            if (!sacrificeBaseEffect.GetComponent<NetworkIdentity>()) sacrificeBaseEffect.AddComponent<NetworkIdentity>();
            sacrificeBaseEffect.GetComponentInChildren<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().blendDistance = 100f;
            sacrificeBaseEffect.transform.Find("AreaIndicator").gameObject.SetActive(false);
            sacrificeBaseEffect.transform.Find("Vacuum Stars, Distortion").gameObject.SetActive(false);
            sacrificeBaseEffect.transform.Find("Point light").gameObject.SetActive(false);
            EffectAPI.AddEffect(sacrificeBaseEffect);

            sacrificeStanEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/effects/ArtifactShellExplosion"), "SacrificeStanEffect", true);
            if (!sacrificeStanEffect.GetComponent<NetworkIdentity>()) sacrificeStanEffect.AddComponent<NetworkIdentity>();
            sacrificeBaseEffect.GetComponentInChildren<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().blendDistance = 100f;
            sacrificeStanEffect.transform.Find("Point light").gameObject.SetActive(false);
            EffectAPI.AddEffect(sacrificeStanEffect);

            weakStanBurnEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/effects/impacteffects/Explosionsolarflare"), "WeakStanBurnEffect", true);
            if (!weakStanBurnEffect.GetComponent<NetworkIdentity>()) weakStanBurnEffect.AddComponent<NetworkIdentity>();
            weakStanBurnEffect.GetComponent<EffectComponent>().soundName = "";
            EffectAPI.AddEffect(weakStanBurnEffect);
        }
    }
}
