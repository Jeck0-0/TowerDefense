using System.Collections.Generic;

namespace TowerDefense
{
    public class UpgradeState
    {
        public int maxWeapons;
        public int maxItems;

        public List<EquippedWeapon> weapons;
 


        public class EquippedWeapon
        {
            public WeaponData weapon;
            public Weapon script;
            public int level;
        }
    }
}