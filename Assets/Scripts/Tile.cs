using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    static public Tile SelectedTile;
    public bool isSelected => SelectedTile == this;
    public Vector2 pos;
    public GridMap map;
    public bool canBuildOver => CanBuildOver();
    public bool enemiesCanWalkOver; //{ get set } change layer thing 

    protected virtual bool CanBuildOver()
    {
        return false;
    }

    public void Remove()
    {
        map.RemoveElement(pos, true);
    }

    static void SelectTile(Tile newSelectedTile)
    {
        newSelectedTile.Select();
    }
    public void Select()
    {
        SelectedTile?.Deselect();
        SelectedTile = this;
        BroadcastMessage("OnSelect");
    }
    public void Deselect()
    {
        if (isSelected)
            SelectedTile = null;
        BroadcastMessage("OnDeselect");
    }
}

