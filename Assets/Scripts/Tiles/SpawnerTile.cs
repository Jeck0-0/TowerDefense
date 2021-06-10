using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerTile : Tile
{
    public SpawnerTile()
    {
        Initialize();
    }
    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        type = TileType.Spawner;
        enemiesCanWalkOver = true;
    }
}
