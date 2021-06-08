using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] GameObject item;
    [SerializeField] Sprite icon;
    [SerializeField] Image imageUI;
    [SerializeField] Text itemNameUI;
    [SerializeField] Text itemPriceUI;

    [SerializeField] string itemName;
    [SerializeField] int price;

    private void Start()
    {
        item.TryGetComponent(out Turret t);
        if (price == 0 && t)
            price = t.cost;
        if (price == -1)
            price = 0;
        if (itemName == string.Empty && t)
            itemName = t.turretName;

        imageUI.sprite = icon;
        itemPriceUI.text = price.ToString("00");
        itemNameUI.text = itemName;
    }

    public void Buy() 
    {
        if (Tile.SelectedTile && Tile.SelectedTile.canBuildOver && 
            Tile.SelectedTile.TryGetComponent(out GroundTile tile)&&
            GameStats.Coins >= price)
        {
            GameStats.Coins -= price;
            tile.BuildTower(item);
        }
            
    }

}
