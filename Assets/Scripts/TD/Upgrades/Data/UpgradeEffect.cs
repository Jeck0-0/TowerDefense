using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace TowerDefense
{
    [Serializable, HideReferenceObjectPicker]
    public abstract class WeaponEffect
    {
        public abstract void Apply(Weapon weapon, TowerUpgrades tower);
        

        [OnInspectorGUI("UpdateTypeLabel"), HideLabel, Space(20)]
        [DisplayAsString(false, fontSize:15, enableRichText:true)]
        [GUIColor(.5f, .8f, 1)] private string type;
        public string UpdateTypeLabel()
        {
            string typeName = GetType().Name.TrimEnd("UpgradeEffect");
            for (int i = typeName.Length - 1; i > 0; i--)
                if (char.IsUpper(typeName[i]))
                    typeName = typeName.Insert(i, " ");
            type = $"   <b>{typeName}</b>";
            return type;
        }
    }
    
    [Serializable]
    public class ModifyTowerStatEffect : WeaponEffect
    {
        public string statName;
        public float add;
        public float multiply = 1;
        
        public override void Apply(Weapon weapon, TowerUpgrades tower)
        {
            tower.Tower.GetStats().SetModifier(statName, "weapon_" + weapon.name, add, multiply, false);
        }
    }
    [Serializable]
    public class ModifyWeaponStatEffect : WeaponEffect
    {
        public string statName;
        public float add;
        public float multiply = 1;
        
        public override void Apply(Weapon weapon, TowerUpgrades tower)
        {
            weapon.GetStats().SetModifier(statName, "weapon_" + weapon.name, add, multiply, false);
        }
    }
    /*[Serializable]
    public class StatWeaponEffect : WeaponEffect
    {
        public string statName;
        public Stat stat;
        
        public override void Apply(Weapon weapon, Tower tower)
        {
            if (weapon.GetStats().HasStat(statName))
            {
                Debug.LogWarning("Weapon already has stat: " + statName, weapon);
                return;
            }
            weapon.GetStats().AddStat(statName, stat);
        }
    }*/
}