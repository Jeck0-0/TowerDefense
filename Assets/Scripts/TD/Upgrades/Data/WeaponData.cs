using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.VisualScripting;
using UnityEngine;

namespace TowerDefense
{
    
    [CreateAssetMenu(menuName = "TowerDefense/Weapon", fileName = "New Weapon")]
    public class WeaponData : SerializedScriptableObject, ITowerUpgrade
    {
        [SerializeField] private CardData cardData;
        [SerializeField] private TowerUpgradeRequirement[] requirements = Array.Empty<TowerUpgradeRequirement>();
        public CardData CardData => cardData;
        public UpgradeCardType UpgradeCardType => UpgradeCardType.Weapon;
        public TowerUpgradeRequirement[] TowerUpgradeRequirements => requirements;

        [ShowInInspector, NonSerialized, OdinSerialize, Required]
        [ValueDropdown(nameof(GetWeapons))]
        public Type WeaponType;
        
        [NonSerialized, OdinSerialize]
        public WeaponUpgrade[] upgrades = Array.Empty<WeaponUpgrade>();
        
        public bool VerifyRequirements(Tower t) => TowerUpgradeRequirements.All(x => x.Verify(t));
        
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
    public class WeaponUpgrade : ITowerUpgrade
    {
        [Title("Upgrade Data", bold: true)]
        [SerializeField, HideReferenceObjectPicker] private CardData cardData = new ();
        [OdinSerialize] private TowerUpgradeRequirement[] requirements = Array.Empty<TowerUpgradeRequirement>();
        public CardData CardData => cardData;
        public UpgradeCardType UpgradeCardType => UpgradeCardType.WeaponUpgrade;
        public TowerUpgradeRequirement[] TowerUpgradeRequirements => requirements;
        
        [SerializeReference]
        public List<WeaponEffect> effects = new List<WeaponEffect>();
        
        public bool VerifyRequirements(Tower t) =>TowerUpgradeRequirements.All(x => x.Verify(t));
    }


    public abstract class Weapon : MonoBehaviour
    {
        
    }

    public class TestWeapon : Weapon
    {
        
    }
}