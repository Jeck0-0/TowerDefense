using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace TowerDefense
{
    public class TowerFactory : Singleton<TowerFactory>
    {
        public Tower SpawnTower(string towerId)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Towers/" + towerId);
            var go = Instantiate(prefab);
            var tower = go.GetComponent<Tower>();
            tower.Initialize();
            
            return tower;
        }
    }
}
