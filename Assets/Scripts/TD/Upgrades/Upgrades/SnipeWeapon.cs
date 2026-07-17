using UnityEngine;

namespace TowerDefense
{
    public class SnipeWeapon : Weapon
    {
        public Stat MaxBoost = new();
        public Stat DamageBoostPerUnit = new();
        
        public override Stats GetStats()
        {
            if(stats != null) return stats;
            var tempStats = base.GetStats();
            tempStats.AddStat("maxBoost", MaxBoost);
            tempStats.AddStat("damageBoostPerUnit", DamageBoostPerUnit);
            return tempStats;
        }


        public override void Awake()
        {
            base.Awake();
            AttackingTower.OnModifyDamage += ModifyDamage;
            
            MaxBoost.SetBaseValue(.3f);
            DamageBoostPerUnit.SetBaseValue(.05f);
        }

        private float ModifyDamage(AttackingTower attacker, GameObject attackSource, ref float currentDamage, Targetable target)
        {
            float dist = Vector2.Distance(attacker.transform.position, target.transform.position);
            float boost = Mathf.Min(MaxBoost, DamageBoostPerUnit * dist);
            currentDamage += currentDamage * boost;
            return currentDamage;
        }

        private void OnDestroy()
        {
            if (AttackingTower)
            {
                AttackingTower.OnModifyDamage -= ModifyDamage;
            }
        }
    }
}