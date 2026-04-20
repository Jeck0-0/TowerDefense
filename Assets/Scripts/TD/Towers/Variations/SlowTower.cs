using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TowerDefense
{
    public class SlowTower : AttackingTower 
    {
        public Stat SlowDuration;
        public Stat SlowAmount;
    
        AudioSource freezeSound;
    
        public override Stats GetStats()
        {
            if (stats != null)
                return stats;
    
            var tempStats = base.GetStats();
            tempStats.AddStat("slowDuration", SlowDuration);
            tempStats.AddStat("slowAmount", SlowAmount);
            return tempStats;
        }
    
        protected override void Attack()
        {
            foreach(Enemy e in GetEnemiesInRange())
            {
                e.Damage(Damage / AttackSpeed.BaseValue);
    
                if (e.EffectHandler.HasEffect("burn")) continue;
                StatModifierEffect slowEffect = new StatModifierEffect("freeze", EffectType.slow, "freeze", e.stats);
                slowEffect.AddModifier("moveSpeed", multiply: SlowAmount);
                e.EffectHandler.AddEffect(slowEffect, SlowDuration);
            }
        }
    
        protected override void OnTargetFound()
        {
            if (freezeSound)
                Destroy(freezeSound.gameObject); 
            freezeSound = AudioController.Instance.PlaySound2D("tower_" + towerID + "_loop", attackSoundVolume, looping: true);
    
        }
    
        protected override void OnTargetLost()
        {
            if(freezeSound)
                Destroy(freezeSound.gameObject);
        }
        private void OnDisable()
        {
            if (freezeSound)
                Destroy(freezeSound.gameObject);
        }
    }
    
}
