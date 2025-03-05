using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class Tile : MonoBehaviour
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


    public TileGFX gfx;

    private void Awake()
    {
        gfx = GetComponent<TileGFX>();
    }

    private void Start()
    {
        if (!GridManager.Instance.Contains(this))
        {
            var coords = GridManager.FixCoordinates(transform.position);
            GridManager.Instance.AddTile(coords, this);
        
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

    public virtual bool CanPlace()
    {
        return CanBuildOver && (Tower == null);
    }

    public virtual BaseDisplayInfo GetDisplayInfo()
    {
        if (tower)
            return tower.GetDisplayInfo();
        
        if(canBuildOver)
            return new ShopDisplayInfo(tileName, description, icon);

        return new BaseDisplayInfo(tileName, description, icon);
    }


    public void PlaceTower(Tower t)
    {
        if (!CanPlace())
            return;
        tower = t;
        t.gameObject.SetActive(true);
        t.gameObject.transform.position = transform.position;
        t.gameObject.transform.parent = transform;
        t.Tile = this;
        name = "Tile [" + t.name + "]";
    }

    public void RemoveTower()
    {
        tower = null;
    }

    private void OnDestroy()
    {
        if(GridManager.Instance && GridManager.Instance.Contains(this))
            GridManager.Instance.Remove(this);
    }
}