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
        public Type[] towerType;

        public override bool Verify(Tower t)
        {
            return towerType.Any(x => t.GetType() == x);
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