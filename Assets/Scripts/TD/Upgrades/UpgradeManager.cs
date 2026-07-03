using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefense
{
    public class UpgradeManager : MonoBehaviour
    {
        [Required] public UpgradeCollection upgrades;

        public int optionsAmount = 3;
        
        private void Awake()
        {
            if (upgrades == null)
                Debug.LogError("No upgrade collection selected!");
        }

        [Button]
        public void ShowTowerUpgrades(Tower t)
        {
            var options = GetOptionsForTower(t);

            foreach (var option in options)
            {
                Debug.Log(option.CardData.Name);
            }
        }


        private List<IUpgradeCard> GetOptionsForTower(Tower t)
        {
            List<IUpgradeCard> candidates = upgrades.weapons
                .Where(x => x && x.WeaponType != null && t.GetComponent(x.WeaponType) == null)
                .ToList<IUpgradeCard>();
            
            // add items to candidates
            
            List<IUpgradeCard> options = new List<IUpgradeCard>();
            for (int i = 0; i < optionsAmount; i++)
            {
                if (candidates.Count <= 0)
                    break;
                
                int r = Random.Range(0, candidates.Count);
                options.Add(candidates.ElementAt(r));
                candidates.RemoveAt(r);
            }
            
            return options;
        }
        
        
        public void ShowGlobalUpgrades()
        {
            
        }
    }
}
