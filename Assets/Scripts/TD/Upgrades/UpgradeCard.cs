using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TowerDefense
{
    public interface IUpgradeCard
    {
        public CardData CardData { get; }
    }

    [Serializable]
    public class CardData
    {
        public string Name;
        public string Description;
        [PreviewField] public Sprite Icon;
    }

    
    
}