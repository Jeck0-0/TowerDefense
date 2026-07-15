using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
    [RequireComponent (typeof (IStatObject))]
    public class HealNearbyEnemies : MonoBehaviour, IStatComponent, IScalable
    {
        public float baseHeal;
        public float addMissingPercent;
        public float missingPercentCap;
        public bool healSelf;


        public Stat HealEffectiveness;
        public Stat HealFrequency;
        public Stat HealRange;

        public Transform healRangeVFX;

        Stats stats;
        private void Awake()
        {
            stats = GetComponent<IStatObject>()?.GetStats();
        }

        float nextHealTime;
        private void Update()
        {
            if(nextHealTime < Time.time)
            {
                nextHealTime = Time.time + 1 / HealFrequency;

                var hit = Physics2D.CircleCastAll(transform.position, HealRange,
                    Vector2.zero, 0, layerMask: LayerMask.GetMask("Targetable"));
                var enemies = hit.Select(x => x.collider.GetComponent<Enemy>()).Where(x => x != null && x.isAlive);

                foreach (Enemy enemy in enemies)
                {
                    if (!healSelf && enemy.gameObject == gameObject)
                        continue;

                    float perc = baseHeal * HealEffectiveness;
                    perc += Mathf.Clamp((enemy.MaxHealth - enemy.currentHealth) * addMissingPercent, 0, missingPercentCap * HealEffectiveness);

                    enemy.Heal(perc);
                }

                healRangeVFX.gameObject.SetActive(false);
                healRangeVFX.gameObject.SetActive(true);
            }
        }

        public void ApplyScaling(Stats stats, float scaling)
        {
            stats.SetModifier("healEffectiveness", "scaling", 0, scaling);
        }

        public Stats GetStats()
        {
            Stats s = new();
            s.AddStat("healRange", HealRange);
            s.AddStat("healFrequency", HealFrequency);
            s.AddStat("healEffectiveness", HealEffectiveness);
            return s;
        }
    }
}
