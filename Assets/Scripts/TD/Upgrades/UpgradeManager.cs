using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefense
{
    public class UpgradeManager : Singleton<UpgradeManager>
    {
        [Required] public UpgradeCollection upgrades;
        public int optionsAmount = 3;

        [Title("UI")]
        [SerializeField] private Transform upgradeUIParent;
        [SerializeField] private GameObject upgradeUIPrefab;
        
        
        private void Awake()
        {
            if (upgrades == null)
                Debug.LogError("No upgrade collection selected!");
            DestroyOptions();
        }

        [Button]
        public void ShowTowerUpgrades(Tower t)
        {
            TowerUpgrades towerUpgrades = t.GetOrAddComponent<TowerUpgrades>();
            var options = GetOptionsForTower(towerUpgrades);

            upgradeUIParent.gameObject.SetActive(true);
            foreach (var option in options)
            {
                var cardUI = Instantiate(upgradeUIPrefab, upgradeUIParent).GetComponent<UpgradeCardUI>();
                cardUI.DisplayCard(option, () => { ChooseTowerUpgrade(towerUpgrades, option); });
            }
        }


        public void ChooseTowerUpgrade(TowerUpgrades t, ITowerUpgrade upgrade)
        {
            DestroyOptions();
            
            var towerUpgrades = t.GetOrAddComponent<TowerUpgrades>();
            towerUpgrades.UnlockUpgrade(upgrade);
            Analytics.Instance?.GetTowerUpgrade(upgrade, t);
        }

        protected void DestroyOptions()
        {
            for (int i = upgradeUIParent.childCount - 1; i > 0 ; i--)
                Destroy(upgradeUIParent.GetChild(i).gameObject);
            upgradeUIParent.gameObject.SetActive(false);
        }


        public bool TowerHasOptions(TowerUpgrades t)
        {
            foreach (var weapon in upgrades.weapons)
            {
                if (t.GetUpgrade(weapon) == null)
                {
                    if (weapon.VerifyRequirements(t))
                        return true;
                }
                else 
                {
                    foreach (var subUpgrade in weapon.upgrades)
                        if (subUpgrade.VerifyRequirements(t))
                            return true;
                }
            }
            return false;
        }
        
        private List<ITowerUpgrade> GetOptionsForTower(TowerUpgrades t)
        {
            List<ITowerUpgrade> candidates = new();
            
            foreach (var weapon in upgrades.weapons)
            {
                if (weapon.WeaponType == null)
                {
                    Debug.LogWarning($"Weapon Type is null! ({weapon.name})", weapon);
                    continue;
                }
                
                if (t.GetUpgrade(weapon) != null)
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
