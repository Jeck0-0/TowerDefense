using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


namespace TowerDefense
{
    public class TowerPlacementManager : Singleton<TowerPlacementManager>
    {
        [ShowInInspector, ReadOnly]
        public Tower placingTower { get; private set; }
        bool instantiateNew;
    
        protected Action onPlace;
        protected Action onCancelPlacing;
    
        protected bool startedPlacingThisFrame;
    
        public void Update()
        {
            if (!placingTower)
                return;
    
            //prevent placing the tower in the same frame it's being selected
            if (startedPlacingThisFrame)
            {
                startedPlacingThisFrame = false;
                return;
            }
    
            //cancel placing
            if (Input.GetKey(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                CancelPlacing();
                return;
            }
    
            //place tower
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                var hit = Physics2D.RaycastAll(
                    Helpers.Camera.ScreenToWorldPoint(Input.mousePosition), 
                    Vector2.zero);
                ITowerSlot slot = null;

                foreach (var h in hit)
                {
                    slot = h.collider.GetComponent<ITowerSlot>();
                    if (slot != null)
                        break;
                }
                
                if (slot != null && slot.CanPlace(placingTower) && !Helpers.IsOverUI)
                {
                    PlaceSelectedTower(slot);
                    return;
                }
            }
        }
    
    
        void PlaceSelectedTower(ITowerSlot tile)
        {
            if (tile == null || !placingTower) return;
    
            InputManager.Instance.SetPlacingStatus(false);
    
            Tower tower = null;
            if (instantiateNew)
            {
                tower = TowerFactory.Instance.SpawnTower(placingTower.towerID);
            }
            else
            {
                tower = placingTower;
            }
    
            tower.gameObject.SetActive(true);
            tower.name = placingTower.name;
            tile.PlaceTower(tower);
            onPlace?.Invoke();
    
            AudioController.Instance.PlaySound2D("ui_confirm");
    
            StopPlacing();
        }
    
    
        public void CancelPlacing()
        {
            onCancelPlacing?.Invoke();
            StopPlacing();
            AudioController.Instance.PlaySound2D("ui_cancel");
        }
    
        protected void StopPlacing()
        {
            placingTower = null;
            onPlace = null;
            onCancelPlacing = null;
    
            TowerPreviewManager.Instance.StopPreviewing();
    
            InputManager.Instance.SetPlacingStatus(false);
        }
    
        public void StartPlacing(Tower tower, Action onPlace = null, Action cancelPlacing = null, bool instantiateNew = true)
        {
            if (placingTower == tower)
            {
                CancelPlacing();
                return;
            }
    
            InputManager.Instance.SetPlacingStatus(true);
            this.instantiateNew = instantiateNew;
            this.placingTower = tower;
            this.onPlace = onPlace;
            this.onCancelPlacing = cancelPlacing;
            startedPlacingThisFrame = true;
    
            TowerPreviewManager.Instance.PreviewTower(tower, false);
    
            //Hide tower
            if (!instantiateNew) 
            {
                tower.transform.position = Vector3.left * 10000;
                tower.Slot?.RemoveTower();
            }
    
            AudioController.Instance.PlaySound2D("ui_confirm");
        }
    }
}
