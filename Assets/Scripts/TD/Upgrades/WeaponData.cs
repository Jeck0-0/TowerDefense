using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TowerDefense
{
    
    [CreateAssetMenu(menuName = "TowerDefense/Weapon", fileName = "New Weapon")]
    public class WeaponData : SerializedScriptableObject, IUpgradeCard
    {
        [SerializeField] private CardData cardData;
        public CardData CardData => cardData;
        
        [ShowInInspector, NonSerialized, OdinSerialize, Required]
        [ValueDropdown(nameof(GetWeapons))]
        public Type WeaponType;
        
        [SerializeReference]
        public List<WeaponUpgrade> upgrades = new List<WeaponUpgrade>();
        
        private static IEnumerable<Type> GetWeapons()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                    typeof(Weapon).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsGenericType);
        }
    }

    [Serializable, HideReferenceObjectPicker]
    public class WeaponUpgrade : IUpgradeCard
    {
        [Title("Upgrade Data", bold: true)]
        [SerializeField] private CardData cardData;
        public CardData CardData => cardData;

        
        [SerializeReference]
        public List<WeaponEffect> effects = new List<WeaponEffect>();
    }


    public abstract class Weapon : MonoBehaviour
    {
        
    }

    public class TestWeapon : Weapon
    {
        
    }
}