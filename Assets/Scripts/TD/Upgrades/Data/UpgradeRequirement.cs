using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TowerDefense
{
    [Serializable, HideReferenceObjectPicker]
    public abstract class TowerUpgradeRequirement
    {
        public abstract bool Verify(TowerUpgrades t, ITowerUpgrade upgrade);
    }


    public class RequireTowerType : TowerUpgradeRequirement
    {
        [ValueDropdown(nameof(GetTowers))]
        public Type[] towerType;

        public override bool Verify(TowerUpgrades t, ITowerUpgrade upgrade)
        {
            return towerType.Any(x => t.GetType() == x);
        }
        
        
        private static IEnumerable<Type> GetTowers()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                    typeof(Tower).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsGenericType);
        }
        
    }
    public class RequireStats : TowerUpgradeRequirement
    {
        public List<string> stats;
        
        public override bool Verify(TowerUpgrades t, ITowerUpgrade upgrade)
        {
            return stats.All(x => t.Tower.GetStats().HasStat(x));
        }
    }
    public class AmountLimit : TowerUpgradeRequirement
    {
        public int maxAmount;
        
        public override bool Verify(TowerUpgrades t, ITowerUpgrade upgrade)
        {
            if (upgrade is not WeaponUpgrade u)
            {
                Debug.LogError("AmountLimit can only be assigned to Weapon Upgrades. Not " + upgrade.CardData.Name, t);
                return true;
            }
            return t.GetUpgrade(u) == null || t.GetUpgrade(u).Level < maxAmount;
        }
    }
}