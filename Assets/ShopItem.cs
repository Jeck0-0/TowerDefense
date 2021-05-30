using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] GameObject tower;
    [SerializeField] Texture2D icon;
    [SerializeField] Image imageUI;
    [SerializeField] Text towerName;

    public void Buy() 
    {
        if (Tile.SelectedTile && Tile.SelectedTile.canBuildOver && 
            Tile.SelectedTile.TryGetComponent(out GroundTile tile))
            tile.BuildTower(tower);
            
    }

}
