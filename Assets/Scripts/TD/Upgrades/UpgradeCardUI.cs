using System.Collections.Generic;
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


        public void DisplayCard(IUpgradeCard card)
        {
            type.text = card.UpgradeCardType.ToString();
        }
    }
}
