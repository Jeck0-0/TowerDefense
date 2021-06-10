using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : Tile
{
    public BaseTile()
    {
        Initialize();
    }
    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        type = TileType.Base;
        enemiesCanWalkOver = true;
    }
}
