using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile : Tile
{
    [SerializeField] GameObject content;
    [SerializeField] GameObject selectEffect;
    
    
    protected override bool CanBuildOver()
    {
        return content == null;
    }

    public GroundTile()
    {
        Initialize();
    }
    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        type = TileType.Ground;
        enemiesCanWalkOver = false;
    }


    private void OnMouseEnter()
    {
        //Debug.Log("brrr enter");    
    }

    private void OnMouseDown()
    {
        if (isSelected)
            Deselect();
        else
            Select();
    }
    private void OnMouseExit()
    {
        //Debug.Log("brrr no mor");
    }

    public void OnDeselect()
    {
        selectEffect.SetActive(false);
        if (!SelectedTile)
            UISideMenu.Instance.Hide();
    }
    public void OnSelect()
    {
        selectEffect.SetActive(true);
        if (canBuildOver)
            UISideMenu.Instance.ShowShop();
        else if (content && content.TryGetComponent(out Turret t))
            UISideMenu.Instance.ShowTurretInfo(t);
    }

    public void BuildTower(GameObject go)
    {
        content = Instantiate(go, transform);
        if (content && content.TryGetComponent(out Turret t))
            UISideMenu.Instance.ShowTurretInfo(t);

        if (content.TryGetComponent(out OnDestroyDispatcher onDestroyScript))
            onDestroyScript.OnObjectDestroyed += OnContentDestroyed;
        else
            content.AddComponent<OnDestroyDispatcher>().OnObjectDestroyed += OnContentDestroyed;
    }

    void OnContentDestroyed(GameObject destroyedThingy)
    {
        if(isSelected)
            UISideMenu.Instance.ShowShop();
    }

    void UpdateSideMenu()
    {
        //// should handle this on side menu script actually
    }

}
