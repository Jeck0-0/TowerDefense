using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace TowerDefense
{
    [Serializable, HideReferenceObjectPicker]
    public abstract class WeaponEffect
    {
        public abstract void Apply(Tower tower);
        

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
    public class ModifyStatUpgradeEffect : WeaponEffect
    {
        public int multiply;
        
        
        public override void Apply(Tower tower)
        {
            Debug.Log(multiply + tower.towerName);
        }
    }
}