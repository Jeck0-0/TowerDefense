using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace TowerDefense
{
    public class TowerFactory : Singleton<TowerFactory>
    {
        [ShowInInspector, ReadOnly]
        private Dictionary<string, List<IUpgrade>> unlockedUpgrades = new();
        
        public Tower SpawnTower(string towerId)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Towers/" + towerId);
            var go = Instantiate(prefab);
            var tower = go.GetComponent<Tower>();
            
            tower.Initialize();
            if(unlockedUpgrades.TryGetValue(towerId, out var unlocked))
                foreach(var upgrade in unlocked)
                    tower.upgradeHandler.UnlockUpgrade("globalUpgrade", upgrade, false);
            if(unlockedUpgrades.TryGetValue("*", out var unlockedGlobal))
                foreach(var upgrade in unlockedGlobal)
                    tower.upgradeHandler.UnlockUpgrade("globalUpgrade", upgrade, false);
            
            return tower;
        }

        public void UnlockUpgrade(string towerId, IUpgrade upgrade)
        {
            if(!unlockedUpgrades.ContainsKey(towerId))
                unlockedUpgrades.Add(towerId, new List<IUpgrade>());
            unlockedUpgrades[towerId].Add(upgrade);
        }
    }
}
