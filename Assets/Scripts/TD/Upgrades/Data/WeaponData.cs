using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
        
        [SerializeReference]
        public List<WeaponEffect> effects = new ();
        
        [NonSerialized, OdinSerialize]
        public WeaponUpgrade[] upgrades = Array.Empty<WeaponUpgrade>();
        
        public bool VerifyRequirements(TowerUpgrades t) => TowerUpgradeRequirements.All(x => x.Verify(t, this));
        public void ApplyUpgrade(TowerUpgrades t)
        {
            var w = t.gameObject.AddComponent(WeaponType) as Weapon;
            w.Initialize(this);
            foreach (var effect in effects)
                effect.Apply(w, t);
        }

        private static IEnumerable<Type> GetWeapons()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                    typeof(Weapon).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsGenericType);
        }

        private void OnEnable()
        {
            foreach (var upgrade in upgrades)
                upgrade.SetWeapon(this);
        }
    }

    [Serializable, HideReferenceObjectPicker]
    public class WeaponUpgrade : ITowerUpgrade
    {
        [Title("Upgrade Data", bold: true)] [SerializeField, HideReferenceObjectPicker]
        private CardData cardData = new();

        [OdinSerialize] private TowerUpgradeRequirement[] requirements = Array.Empty<TowerUpgradeRequirement>();
        public CardData CardData => cardData;
        public UpgradeCardType UpgradeCardType => UpgradeCardType.WeaponUpgrade;
        public TowerUpgradeRequirement[] TowerUpgradeRequirements => requirements;

        [SerializeReference] public List<WeaponEffect> effects = new();

        public WeaponData Weapon { get; private set; }
        internal void SetWeapon(WeaponData weapon) => Weapon = weapon;

        public bool VerifyRequirements(TowerUpgrades t) => TowerUpgradeRequirements.All(x => x.Verify(t, this));
        public void ApplyUpgrade(TowerUpgrades t)
        {
            var w = (Weapon)t.GetComponent(Weapon.WeaponType);
            w.LevelUp();
            foreach (var effect in effects)
                effect.Apply(w, t);
        }
    }


    public abstract class Weapon : MonoBehaviour
    {
        public int Level { get; private set; }
        public void LevelUp() => Level++;
        protected Tower tower;
        protected AttackingTower attackingTower => tower as AttackingTower;
        
        protected Stats stats;
        public virtual Stats GetStats()
        {
            return stats ?? new Stats();
        }
        
        public WeaponData WeaponData { get; private set; }

        public void Initialize(WeaponData data)
        {
            WeaponData = data;
        }
        
        public virtual void Awake()
        {
            tower = GetComponent<Tower>();
        }


        //awake is called before upgrade effects are applied
        //start is called after upgrade effects are applied

    }
}