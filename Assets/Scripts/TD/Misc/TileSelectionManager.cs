using Sirenix.OdinInspector;
using System;
using UnityEngine;


namespace TowerDefense
{
    public class TileSelectionManager : Singleton<TileSelectionManager>
    {
        public Tile SelectedTile => selectedTile;
        [ShowInInspector, ReadOnly] Tile selectedTile;
    
        [SerializeField] Transform selectionIndicator;
    
        private Action<Tile> specialSelectAction;
        private bool selectTileAfterSpecialAction;
    
        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                if (Helpers.IsOverUI)
                    return;
    
                Tile hovering = GridManager.Instance.GetHoveringTile();
    
                if(!hovering || hovering == selectedTile)
                    DeselectTile();
                else
                    SelectTile(hovering);
            }
    
            if (Input.GetMouseButtonDown(1))
                DeselectTile();
        }
    
        public void DeselectTile()
        {
            selectionIndicator.gameObject.SetActive(false);
            DisplayInfoUI.Instance?.Hide();
            
            if (!selectedTile)
                return;
            SetTileDeselected(selectedTile);
            selectedTile = null;
        }
        
        void SelectTile(Tile tile)
        {
            if(selectedTile)
                SetTileDeselected(selectedTile);

            if(specialSelectAction != null)
            {
                specialSelectAction(tile);
                specialSelectAction = null;
    
                if (!selectTileAfterSpecialAction)
                    return;
            }
            
            selectionIndicator.gameObject.SetActive(true);
            selectionIndicator.position = tile.transform.position;
    
            DisplayInfoUI.Instance?.Show(tile.GetDisplayInfo());
    
            selectedTile = tile;
            tile.Tower?.OnTileSelected();
        }

        void SetTileDeselected(Tile tile)
        {
            tile?.Tower?.OnTileDeselected();
        }        
    
        public void SetSpecialSelectAction(Action<Tile> specialSelectAction, bool selectAfterAction)
        {
            this.specialSelectAction = specialSelectAction;
            this.selectTileAfterSpecialAction = selectAfterAction;
        }
    
    }
    
}
