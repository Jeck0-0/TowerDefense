using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    static public Tile SelectedTile;
    public bool selected => SelectedTile == this;
    public Vector2 pos;
    public GridMap map;
    public bool canBuildOver;
    public bool enemiesCanWalkOver; //{ get set } change layer thing 

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
        if (selected)
            SelectedTile = null;
        BroadcastMessage("OnDeselect");
    }
}

