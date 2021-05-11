using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Projectile;
using R2API;


namespace StanWorshipper.Survivor
{
    internal static class Projectiles
    {
        public static GameObject poopslingerProjectile;

        public static void Initialize()
        {
            poopslingerProjectile = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/ClayPotProjectile"), "PoopslingerProjectile", true);
            if (!poopslingerProjectile.GetComponent<NetworkIdentity>()) poopslingerProjectile.AddComponent<NetworkIdentity>();
            poopslingerProjectile.GetComponent<ProjectileSimple>().velocity = 100f;
            poopslingerProjectile.GetComponent<ProjectileController>().procCoefficient = 1.5f;
            poopslingerProjectile.GetComponent<ProjectileController>().ghostPrefab = Resources.Load<GameObject>("prefabs/projectileghosts/BeetleQueenSpitGhost");
            poopslingerProjectile.GetComponent<ProjectileDamage>().damage = 1f;
            poopslingerProjectile.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
            poopslingerProjectile.GetComponentInChildren<ProjectileImpactExplosion>().blastRadius = 6.5f;
            poopslingerProjectile.GetComponentInChildren<ProjectileImpactExplosion>().impactEffect = Effects.poopslingerExplosionEffect;

            ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(poopslingerProjectile);
            };
        }
    }
}
