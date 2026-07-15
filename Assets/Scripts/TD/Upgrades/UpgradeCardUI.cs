using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class UpgradeCardUI : MonoBehaviour
    {
        public TMP_Text title;
        public TMP_Text description;
        public TMP_Text type;
        public Image icon;
        private Action action;

        
        public void DisplayCard(IUpgradeCard card, Action action)
        {
            title.text = card.CardData.Name;
            description.text = card.CardData.Description;
            icon.sprite = card.CardData.Icon;
            type.text = card.UpgradeCardType.ToString();
            this.action = action;
        }


        public void OnClick()
        {
            action?.Invoke();
        }
    }
}
