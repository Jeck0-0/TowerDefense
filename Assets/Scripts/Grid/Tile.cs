using Sirenix.OdinInspector;
using System;
using UnityEngine;


namespace TowerDefense
{
    [Serializable]
    public class Tile : MonoBehaviour, ITowerSlot
    {
        public enum TileType { path, ground, decoration }
        
        public string tileId;
    
        [SerializeField] Sprite icon;
        [SerializeField] string tileName;
        [SerializeField] string description;
        [SerializeField] protected TileType type;
    
        [SerializeField] bool isWalkable;
        [SerializeField] bool isHome;
        [SerializeField] bool canBuildOver;
    
        [SerializeField, ReadOnly] Vector2Int position;
        [SerializeField] Tower tower;
    
        public TileType Type => type;
        public Vector2Int Position => position;
        public bool IsWalkable => isWalkable;
        public bool IsHome => isHome;
        public bool CanBuildOver => canBuildOver;
        public Tower Tower => tower;
    
        //called when the tile is moved or removed
        public event Action OnTileChanged;
    
        public TileGFX gfx;
    
        private void Awake()
        {
            gfx = GetComponent<TileGFX>();
            name = tileName;
        }
    
        private void Start()
        {
            if (!GridManager.Instance.Contains(this))
            {
                /*var coords = GridManager.FixCoordinates(transform.position);
                GridManager.Instance.AddTile(coords, this);*/
            
                var t = GetComponentInChildren<Tower>();
                if(t != null)
                    PlaceTower(t);
            }
        }
    
        public void InitializeTile(Vector2Int position)
        {
            this.position = position;
            transform.parent = GridManager.Instance.transform;
            transform.position = position.ToVector3();
            OnNearbyTileChanged();
        }
    
        public virtual void OnNearbyTileChanged() 
        {
            gfx.UpdateGFX();
        }
    
        public virtual bool CanPlace(Tower t)
        {
            return CanBuildOver && !Tower;
        }
    
        public virtual BaseDisplayInfo GetDisplayInfo()
        {
            if (tower)
                return tower.GetDisplayInfo();
            
            if(canBuildOver)
                return new ShopDisplayInfo(tileName, description, icon);
    
            return new BaseDisplayInfo(tileName, description, icon);
        }
        

        public virtual void PlaceTower(Tower t)
        {
            if (!CanPlace(t))
                return;
            tower = t;
            t.gameObject.SetActive(true);
            t.gameObject.transform.position = transform.position;
            t.gameObject.transform.parent = transform;
            t.SetSlot(this);
            name = $"{tileName} [" + t.towerName + "]";
        } 
    
        public virtual void RemoveTower()
        {
            tower = null;
            if(TileSelectionManager.Instance?.SelectedTile == this)
            {
                //DisplayInfoUI.Instance.UpdateInfo();
                DisplayInfoUI.Instance?.Hide();
                TileSelectionManager.Instance?.DeselectTile();
            }
            name = tileName;
        }
    
        private void OnDestroy()
        {
            if(GridManager.Instance && GridManager.Instance.Contains(this))
                GridManager.Instance.Remove(this);
            OnTileChanged?.Invoke();
        }
    }
}
