using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTile : Tile
{
    public PathTile()
    {
        Initialize();
    }
    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        type = TileType.Path;
        enemiesCanWalkOver = true;
    }
}
