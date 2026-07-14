using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
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


        public void UnlockTowerUpgrade(Tower t, ITowerUpgrade upgrade)
        {
            upgrade.ApplyUpgrade(t);
        }


        private List<ITowerUpgrade> GetOptionsForTower(Tower t)
        {
            List<ITowerUpgrade> candidates = new();

            foreach (var weapon in upgrades.weapons)
            {
                if (weapon.WeaponType == null)
                {
                    Debug.LogWarning($"Weapon Type is null! ({weapon.name})", weapon);
                    continue;
                }
                
                if (t.GetComponent(weapon.WeaponType) != null)
                {
                    // already has upgrade
                    foreach (var subUpgrade in weapon.upgrades)
                        if (subUpgrade.VerifyRequirements(t))
                            candidates.Add(subUpgrade);
                }
                else
                {
                    // is unequipped weapon
                    if (weapon.VerifyRequirements(t))
                        candidates.Add(weapon);
                }
            }
            
            // add items to candidates
            
            List<ITowerUpgrade> options = new List<ITowerUpgrade>();
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
