using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    [RequireComponent(typeof(Tower))]
    public class TowerUpgrades : MonoBehaviour
    {
        public float upgradeCostMultiplier = 1.5f;
        
        public Tower Tower => tower ?? GetComponent<Tower>();
        private Tower tower;

        private GameObject upgradeIcon;
        
        private Dictionary<WeaponData, Weapon> unlockedWeapons = new();
        
        private void Awake()
        {
            tower = GetComponent<Tower>();

            var iconPrefab = Resources.Load<GameObject>("Prefabs/UI/UpgradableIcon");
            upgradeIcon = Instantiate(iconPrefab, transform.position, transform.rotation, transform);
            upgradeIcon.SetActive(false);
            UpdateUpgradeIcon();
            GameStats.Instance.coinsChanged += UpdateUpgradeIcon;
        }

        private void OnDestroy()
        {
            if(GameStats.Instance) GameStats.Instance.coinsChanged -= UpdateUpgradeIcon;
        }

        private bool canUpgrade;
        private void UpdateUpgradeIcon()
        {
            if (canUpgrade != CanUpgrade())
            {
                canUpgrade = GameStats.Instance.coins >= Tower.UpgradeCost;
                upgradeIcon.SetActive(canUpgrade);
            }
        }

        private bool CanUpgrade()
        {
            if (GameStats.Instance.coins < Tower.UpgradeCost) 
                return false;
            if (!UpgradeManager.Instance?.TowerHasOptions(this) ?? false)
                return false;
            return true;
        }

        public void TryUnlockUpgrade()
        {
            if (CanUpgrade())
            {
                GameStats.Instance.ModifyCoins(-(int)Tower.UpgradeCost);
                UpgradeManager.Instance?.ShowTowerUpgrades(Tower);
                Tower.UpgradeCost.SetModifier("upgradeMultiplier", 0, upgradeCostMultiplier, false);
            }
        }
        
        
        public UnlockedWeaponUpgrade GetUpgrade(WeaponUpgrade upgrade)
        {
            return GetUpgrade(upgrade.Weapon)?.GetUpgrade(upgrade);
        }
        public Weapon GetUpgrade(WeaponData upgrade)
        {
            unlockedWeapons.TryGetValue(upgrade, out var u);
            return u;
        }
        
        public void UnlockUpgrade(ITowerUpgrade towerUpgrade)
        {
            if (towerUpgrade is WeaponData weapon)
            {
                var component = gameObject.AddComponent(weapon.WeaponType);
                unlockedWeapons.Add(weapon, component as Weapon);
            }
            else if (towerUpgrade is WeaponUpgrade upgrade)
            {
                if (!unlockedWeapons.ContainsKey(upgrade.Weapon))
                {
                    Debug.LogError("Tower has no upgrade of type: " + upgrade.Weapon.name, this);
                    return;
                }

                
            }
            
            //Apply
            towerUpgrade.ApplyUpgrade(this);
        }
        
        
    }
}
