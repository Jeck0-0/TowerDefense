using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class ExecuteWeapon : Weapon
    {
        public Stat ExecuteAtPercentHp = new();

        public override Stats GetStats()
        {
            if(stats != null) return stats;
            var tempStats = base.GetStats();
            tempStats.AddStat("executeAtPercentHp", ExecuteAtPercentHp);
            return tempStats;
        }


        public override void Awake()
        {
            base.Awake();
            AttackingTower.OnDamageEvent += OnDamage;
            ExecuteAtPercentHp.SetBaseValue(.05f);
        }

        private void OnDamage(AttackingTower attacker, Targetable target, float originalAmount, float unblockedAmount, bool killed)
        {
            if (target.currentHealth / target.MaxHealth < ExecuteAtPercentHp)
            {
                target.Kill();
            }
        }
    }
}