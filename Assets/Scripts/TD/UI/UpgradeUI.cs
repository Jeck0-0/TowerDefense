using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class UpgradeUI : MonoBehaviour
    {
        public TowerUpgradeSeriesSO upgradeSeries;

        public TMP_Text description;
        public TMP_Text price;
        public TMP_Text title;
        public Image icon;

        private int upgradeIndex;
        private TowerUpgrade upgrade;

        private void Start()
        {
            GameStats.Instance.coinsChanged += UpdatePriceColor;
            upgrade = upgradeSeries.GetUpgrade(0);
            UpdateUI();
        }

        private void OnDestroy()
        {
            if (GameStats.Instance)
                GameStats.Instance.coinsChanged -= UpdatePriceColor;
        }

        void UpdatePriceColor()
        {
            price.color = upgrade.cost <= GameStats.Instance.coins ? TDColors.AffordableColor : TDColors.UnaffordableColor;
        }
        
        protected void UpdateUI()
        {
            description.text = upgrade.description;
            price.text = upgrade.cost.ToString();
            icon.sprite = upgradeSeries.upgradeIcon;
            title.text = $"[{upgradeIndex + 1}] " + upgradeSeries.upgradeTitle;
            UpdatePriceColor();
            //icon.sprite = upgrade.
        }

        public void BuyUpgrade()
        {
            if (upgradeIndex >= upgradeSeries.Count)
            {
                Debug.LogWarning("All upgrades have been unlocked");
                return;
            }
            
            if (upgrade.cost > GameStats.Instance.coins)
                return;
            
            GameStats.Instance.ModifyCoins(-upgrade.cost);
            
            foreach (var u in upgrade.upgrade)
                GameManager.Instance.UnlockGlobalUpgrade(upgrade.targetTower, u);
            
            upgradeIndex++;
            if (upgradeIndex >= upgradeSeries.Count)
            {
                Destroy(gameObject);
                return;
            }
            
            upgrade = upgradeSeries.GetUpgrade(upgradeIndex);
            UpdateUI();
        }
    }
}
