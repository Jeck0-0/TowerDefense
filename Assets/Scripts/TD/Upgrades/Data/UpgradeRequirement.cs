using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace TowerDefense
{
    [Serializable, HideReferenceObjectPicker]
    public abstract class TowerUpgradeRequirement
    {
        public abstract bool Verify(Tower t);
    }


    public class RequireTowerType : TowerUpgradeRequirement
    {
        [ValueDropdown(nameof(GetTowers))]
        public Type[] towerType;

        public override bool Verify(Tower t)
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
        
        public override bool Verify(Tower t)
        {
            return stats.All(x => t.GetStats().HasStat(x));
        }
    }
}