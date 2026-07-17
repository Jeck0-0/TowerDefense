using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class MinigunWeapon : Weapon
    {
        public Stat MaxBoost = new ();
        public Stat BoostIncreaseStep = new ();
        
        
        private float currentBoost;
        private bool lostTarget;

        public override Stats GetStats()
        {
            if(stats != null) return stats;
            var tempStats = base.GetStats();
            tempStats.AddStat("maxBoost", MaxBoost);
            tempStats.AddStat("boostIncreaseStep", BoostIncreaseStep);
            return tempStats;
        }


        public override void Awake()
        {
            base.Awake();
            AttackingTower.OnAttack += OnAttack;
            AttackingTower.OnTargetLostEvent += OnTargetLost;
            
            MaxBoost.SetBaseValue(1f);
            BoostIncreaseStep.SetBaseValue(.1f);
        }

        private void OnDestroy()
        {
            if (AttackingTower)
            {
                AttackingTower.OnAttack -= OnAttack;
                AttackingTower.OnTargetLostEvent -= OnTargetLost;
            }
        }

        private void LateUpdate()
        {
            if (lostTarget && AttackingTower.target == null)
            {
                currentBoost = 0;
                AttackingTower.GetStats().RemoveModifier("attackSpeed", "weapon_minigun");
            }
            lostTarget = false;
        }

        private void OnTargetLost(AttackingTower attacker, Targetable previousTarget)
        {
            lostTarget = true;
        }

        private void OnAttack(AttackingTower attacker, Targetable primaryTarget, IEnumerable<Targetable> allTargets)
        {
            currentBoost = Mathf.Min(currentBoost + BoostIncreaseStep, MaxBoost);
            AttackingTower.GetStats().SetModifier("attackSpeed", "weapon_minigun", multiply: currentBoost + 1,
                overrideIfDuplicate: true);
        }
        
        
    }
}
