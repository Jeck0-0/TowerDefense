using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using UnityEngine;


namespace TowerDefense
{
    public class Targetable : ManagedBehaviour, IStatObject
    {
        [HideInEditorMode]
        public float currentHealth = 0;
        public bool isAlive { private set; get; } = true;
    
        public Stat MaxHealth;
        public Stat DamageTakenMultiplier;
    
        public event Action HealthChanged;
        public event Action<OnDeathArgs> OnDeath;
    
        [HideInInspector]
        public Stats stats;
    
    
        protected override void ManagedInitialize()
        {
            stats = GetStats();
    
            currentHealth = MaxHealth;
            MaxHealth.OnValueChanged += OnMaxHealthChanged;
        }
    
        public virtual Stats GetStats() 
        {
            if (stats != null) return stats;

            var tempStats = new Stats();
            tempStats.AddStat("maxHealth", MaxHealth);
            tempStats.AddStat("damageTakenMultiplier", DamageTakenMultiplier, 0);

            var components = GetComponents<IStatComponent>();
            foreach (var component in components)
            {
                var s = component.GetStats();
                s.stats.ForEach(x => tempStats.AddStat(x.Value.Name, x.Value));
            }

            return tempStats;
        }
    
        protected void OnMaxHealthChanged(Stat.StatValueChangedEventArgs args)
        {
            currentHealth = currentHealth / args.previousValue * args.newValue;
        }
    
        public record DamageReport(float Dealt, bool Killed);
        public DamageReport Damage(float amount)
        {
            if (amount <= 0)
                return new DamageReport(0, false);

            float dmg = amount * stats["damageTakenMultiplier"];
            
            if (dmg + 0.001f > currentHealth)
            {
                currentHealth = 0;
                HealthChanged?.Invoke();
                Kill();
                return new DamageReport(currentHealth, true);
            }

            currentHealth -= dmg;
            HealthChanged?.Invoke();
            return new DamageReport(dmg, false);
        }

    
        public void Heal(float amount)
        {
            if (amount <= 0)
                return;
    
            currentHealth += amount;
    
            HealthChanged?.Invoke();
    
            if (currentHealth > MaxHealth)
                currentHealth = MaxHealth;
        }

        public void Kill() => Kill(false);
        protected void Kill(bool reachedHomeTile)
        {
            isAlive = false;
            OnDeath?.Invoke(new OnDeathArgs() { reachedHomeTile = reachedHomeTile });
            OnKill();
        }

        protected virtual void OnKill()
        {
            Debug.Log(name);
            Destroy(gameObject);
        }

        public class OnDeathArgs
        {
            public bool reachedHomeTile;
        }
    }
}
