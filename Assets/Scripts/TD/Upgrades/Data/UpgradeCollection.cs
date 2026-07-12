using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "New UpgradeCollection", menuName = "TowerDefense/UpgradeCollection")]
    public class UpgradeCollection : ScriptableObject
    {
        public List<WeaponData> weapons;
        
    }
}