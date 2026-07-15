using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace TowerDefense
{
    [RequireComponent(typeof(Tower))]
    public class TowerUpgrades : MonoBehaviour
    {
        public Tower Tower => tower ?? GetComponent<Tower>();

        private Tower tower;
        private Dictionary<WeaponData, UnlockedWeaponData> unlockedWeapons;
        
        private void Awake()
        {
            tower = GetComponent<Tower>();
        }

        
        public UnlockedWeaponUpgradeData GetUpgrade(WeaponUpgrade upgrade)
        {
            UnlockedWeaponUpgradeData u = null;
            GetUpgrade(upgrade.Weapon)?.unlockedUpgrades.TryGetValue(upgrade, out u);
            return u;
        }
        public UnlockedWeaponData GetUpgrade(WeaponData upgrade)
        {
            unlockedWeapons.TryGetValue(upgrade, out var u);
            return u;
        }
        
        public void UnlockUpgrade(ITowerUpgrade towerUpgrade)
        {
            if (towerUpgrade is WeaponData weapon)
            {
                var component = gameObject.AddComponent(weapon.WeaponType);
                unlockedWeapons.Add(weapon, new UnlockedWeaponData(component as Weapon));
            }
            else if (towerUpgrade is WeaponUpgrade upgrade)
            {
                if (!unlockedWeapons.ContainsKey(upgrade.Weapon))
                {
                    Debug.LogError("Tower has no upgrade of type: " + upgrade.Weapon.name, this);
                    return;
                }

                var data = GetUpgrade(upgrade);
                if (data == null)
                    GetUpgrade(upgrade.Weapon).unlockedUpgrades.Add(upgrade, new UnlockedWeaponUpgradeData());
                else
                    data.LevelUp();
            }
            
            //Apply
            towerUpgrade.ApplyUpgrade(this);
        }
        
        


        public class UnlockedWeaponData
        {
            public Weapon weapon;
            public Dictionary<WeaponUpgrade, UnlockedWeaponUpgradeData> unlockedUpgrades = new();
            public int Level => unlockedUpgrades.Sum(x => x.Value.Level);

            public UnlockedWeaponData(Weapon weapon)
            {
                this.weapon = weapon;
            }
        }

        public class UnlockedWeaponUpgradeData
        {
            public int Level { get; private set; } = 1;
            public void LevelUp() => Level++;
        }
    }
}
