using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace TowerDefense
{
    public class ProjectileTower : AttackingTower
    {
        [Header("Projectile")]
        public GameObject projectilePrefab;
        public Transform bulletSpawnpoint;
        public bool destroyProjectileOnTargetDeath = true;
    
        public Stat ProjectileSpeed;
        public Stat SplashDamageArea;
        public Stat ProjectileLifetime;
    
        [BoxGroup("Sound")]
        [Range(0f, 1f)]
        public float projectileHitSoundVolume = .5f;
    
        public override Stats GetStats()
        {
            if (stats != null)
                return stats;
    
            var tempStats = base.GetStats();
            tempStats.AddStat("projectileSpeed", ProjectileSpeed);
            tempStats.AddStat("splashDamageArea", SplashDamageArea);
            tempStats.AddStat("projectileLifetime", ProjectileLifetime);
            return tempStats;
        }
    
        protected override void Attack()
        {
            GameObject go = Instantiate(projectilePrefab, bulletSpawnpoint.position, bulletSpawnpoint.rotation);
            string hitSFX = $"tower_{towerID}_hit";
            go.GetComponent<Projectile>().Initialize(Damage, ProjectileSpeed, ProjectileLifetime, SplashDamageArea, target, destroyProjectileOnTargetDeath, hitSFX, projectileHitSoundVolume);
        }
    }
}
