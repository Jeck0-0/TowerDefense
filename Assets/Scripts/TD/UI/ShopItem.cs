using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TowerDefense
{
    public class ShopItem : MonoBehaviour
    {
        public GameObject prefab;
        public Image iconUI;
        public TMP_Text costUI;
        public TMP_Text towerNameUI;
        public TMP_Text towerDescUI;

        public bool disablePrice = false;
        
        protected Tower tower;

        [ShowInInspector, ReadOnly] private float currentCostMultiplier = 1;
        
        protected void Awake()
        {
            tower = prefab.GetComponent<Tower>();
            iconUI.sprite = tower.shopIcon;
            costUI.text = ((int)tower.Cost).ToString();
            towerNameUI.text = tower.towerName;
            towerNameUI.color = tower.towerColor;
            towerDescUI.text = tower.towerDescription;
            towerDescUI.color = new Color(tower.towerColor.r, tower.towerColor.g, tower.towerColor.b, .5f);
            GameStats.Instance.coinsChanged += UpdatePriceUI;
            tower.Cost.OnValueChanged += CostChanged;
            UpdatePriceUI();
        }
    
        public void Select()
        {
            //BuyTower();
            //If already placing, deselect instead
            if (TowerPlacementManager.Instance.placingTower == tower)
            {
                TowerPlacementManager.Instance.CancelPlacing();
                return;
            }
            //Check input is not occupied (for example playing a potion or a different tower)
            if (InputManager.Instance && !InputManager.Instance.acceptInput)
                return;
            //Check player has enough money
            if (GameStats.Instance && GameStats.Instance.coins < GetCost())
                return;
    
            //TOGGLE SELECT
            TowerPlacementManager.Instance.StartPlacing(tower, OnTowerPlaced);
    
            if (TowerPlacementManager.Instance.placingTower)
                AudioController.Instance.PlaySound2D("ui_confirm");
            else
                AudioController.Instance.PlaySound2D("ui_cancel");
        }
    
        public void OnCursorHover()
        {
            //if (TileSelectionManager.Instance.SelectedTile)
            //    TowerPreviewManager.Instance.PreviewTower(tower, TileSelectionManager.Instance.SelectedTile.Position, placeCondition: () => { return GameStats.Instance.coins >= tower.Cost; });
            //DisplayInfoUI.Instance?.Preview(tower.GetDisplayInfo());
            
            
            //DisplayInfoUI.Instance.Show(this, tower.shopIcon, tower.towerName, tower.towerDescription, false, tower.GetStats(), tower.upgradeHandler);
        }
        public void OnCursorExit()
        {
            //TowerPreviewManager.Instance.StopPreviewing();
            //DisplayInfoUI.Instance?.StopPreview();
            //DisplayInfoUI.Instance.Hide(this);
        }
    
        protected void OnTowerPlaced()
        {
            GameStats.Instance?.ModifyCoins(-GetCost());
            currentCostMultiplier *= tower.costMultiplier;
            UpdatePriceUI();
        }

        protected int GetCost() => disablePrice ? 0 : Mathf.RoundToInt(tower.Cost * currentCostMultiplier);
        
        protected void BuyTower()
        {
            if (GameStats.Instance && GameStats.Instance.coins < GetCost())
                return;
            
            if (TileSelectionManager.Instance.SelectedTile && TileSelectionManager.Instance.SelectedTile.Tower == null)
            {
                OnTowerPlaced();
                
                var t = TowerFactory.Instance.SpawnTower(tower.towerID);
                TileSelectionManager.Instance.SelectedTile.PlaceTower(t);

                TowerPreviewManager.Instance.StopPreviewing();
                DisplayInfoUI.Instance?.Show(TileSelectionManager.Instance.SelectedTile.GetDisplayInfo());
            }
        }
    
        void CostChanged(object args) => UpdatePriceUI();
        
        protected void UpdatePriceUI()
        {
            costUI.text = GetCost().ToString();
            if (GetCost() <= GameStats.Instance.coins)
                costUI.color = TDColors.AffordableColor;
            else
                costUI.color = TDColors.UnaffordableColor;
        }

        private void OnDestroy()
        {
            if(GameStats.Instance)
                GameStats.Instance.coinsChanged -= UpdatePriceUI;
            if(tower)
                tower.Cost.OnValueChanged -= CostChanged;
        }
    }
    
}
