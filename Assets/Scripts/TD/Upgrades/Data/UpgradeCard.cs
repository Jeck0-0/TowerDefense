using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TowerDefense
{
    public interface IUpgradeCard
    {
        public CardData CardData { get; }
        public UpgradeCardType UpgradeCardType { get; }
    }

    public interface ITowerUpgrade : IUpgradeCard
    {
        public TowerUpgradeRequirement[] TowerUpgradeRequirements { get; }

        public bool VerifyRequirements(TowerUpgrades t);
        public void ApplyUpgrade(TowerUpgrades t);
    }

    [Serializable]
    public class CardData
    {
        public string Name;
        public string Description;
        [PreviewField] public Sprite Icon;
    }

    public enum UpgradeCardType { Weapon, WeaponUpgrade, Item, ItemUpgrade }
    
}