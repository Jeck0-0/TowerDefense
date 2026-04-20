using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using System;


namespace TowerDefense
{
    public class TowerPreviewManager : Singleton<TowerPreviewManager>
    {
        [ShowInInspector, ReadOnly]
        public Tower previewingTower { get; private set; }
        Vector2Int startPosition;
        Func<bool> placeCondition;
    
        bool usingOriginalGFX;
        bool followCursor;
        bool isPreviewing;
    
        protected GameObject preview;
        protected SpriteRenderer[] previewRenderers;
        protected RangeIndicator rangePreview;
    
        private void Awake()
        {
            rangePreview = Instantiate(Resources.Load("Prefabs/UI/RangePreview"), transform).GetComponent<RangeIndicator>();
            rangePreview.scaleWithStat.multiply = 2;
            rangePreview.gameObject.SetActive(false);
        }
    
    
        public void Update()
        {
            if (!isPreviewing) 
                return;
    
            UpdatePreview();
        }
    
        void UpdatePreview()
        {
            ITowerSlot slot = null;
            var mousePos = Helpers.Camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int coords = followCursor ? GridManager.FixCoordinates(mousePos) : startPosition;
            Vector3 pos = coords.ToVector3();
            if (followCursor)
            {
                var hit = Physics2D.RaycastAll(
                    mousePos, Vector2.zero);
                
                foreach (var h in hit)
                {
                    slot = h.collider.GetComponent<ITowerSlot>();
                    if (slot != null)
                    {
                        if (slot is MonoBehaviour gateSlot)
                        {
                            pos = gateSlot.transform.position; 
                        }
                        break;
                    }
                }
            }
            else
            {
                slot = GridManager.Instance.Get(coords);
                if(slot != null)
                    pos = ((Tile)slot).transform.position;
            }
            
            if (followCursor)
            {
                rangePreview.transform.position = pos;
                preview.transform.position = pos;
            }

            bool conditionMet = placeCondition?.Invoke() ?? true;
            if (slot != null && conditionMet && slot.CanPlace(previewingTower))
            {
                previewRenderers.ForEach(x => x.color = Color.white);
                if(slot.HideRangePreview)
                    rangePreview.Hide();
                else
                    rangePreview.ShowColor(previewingTower.towerColor);
            }
            else
            {
                previewRenderers.ForEach(x => x.color = Color.red);
                if(slot != null && slot.HideRangePreview)
                    rangePreview.Hide();
                else
                    rangePreview.ShowColor(Color.red);
            }
        }
    
        public void StopPreviewing()
        {
            isPreviewing = false;
    
            if (preview)
            {
                if (usingOriginalGFX)
                {
                    preview.transform.localPosition = Vector3.zero;
                    previewRenderers.ForEach(x => x.color = Color.white);
                    rangePreview.ShowColor(previewingTower.towerColor);
                }
                else
                    Destroy(preview);
            }
    
            rangePreview.gameObject.SetActive(false);
        }
    
        public void PreviewTower(Tower tower, bool useOriginalGFX = false, Func<bool> placeCondition = null)
        {
            var pos = GridManager.FixCoordinates(Helpers.Camera.ScreenToWorldPoint(Input.mousePosition));
            PreviewTower(tower, pos, true, useOriginalGFX, placeCondition);
        }
    
        public void PreviewTower(Tower tower, Vector2Int position, bool useOriginalGFX = false, Func<bool> placeCondition = null)
            => PreviewTower(tower, position, false, useOriginalGFX, placeCondition);
    
        private void PreviewTower(Tower tower, Vector2Int position, bool followCursor, bool useOriginalGFX, Func<bool> placeCondition)
        {
            if (isPreviewing)
                StopPreviewing();
    
            this.previewingTower = tower;
            this.followCursor = followCursor;
            this.startPosition = position;
            this.usingOriginalGFX = useOriginalGFX;
            this.placeCondition = placeCondition;
            this.isPreviewing = true;
    
            //Range preview
            var pos = new Vector3(position.x, position.y, rangePreview.transform.position.z);
            rangePreview.transform.position = pos;
            rangePreview.scaleWithStat.SetStat(tower.MaxRange);
            rangePreview.gameObject.SetActive(true);
            rangePreview.ShowColor(tower.towerColor);
    
            //Tower preview
            var gfx = tower.transform.Find("GFX");
            if (useOriginalGFX)
                preview = gfx.gameObject;
            else
                preview = Instantiate(gfx, pos, Quaternion.identity).gameObject;
    
            previewRenderers = preview.GetComponentsInChildren<SpriteRenderer>();
        }
    
    
    }
    
}
