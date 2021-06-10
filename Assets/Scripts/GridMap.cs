using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    public Dictionary<Vector2, Tile> grid;

    private void Awake()
    {
        grid = new Dictionary<Vector2, Tile>();
        Tile[] tiles = GetComponentsInChildren<Tile>();
        foreach (Tile t in tiles)
            AddElement(t.pos, t.gameObject);
    }

    public List<Tile> GetNeighbours(Tile tile)
    {
        List<Tile> neighbours = new List<Tile>(); 

        Vector2 position = 
                   new Vector2(tile.pos.x + 1, tile.pos.y); //looks like chunky shit but does the job
        if (grid.ContainsKey(position))
            neighbours.Add(grid[position]);
        position = new Vector2(tile.pos.x - 1, tile.pos.y);
        if (grid.ContainsKey(position))
            neighbours.Add(grid[position]);
        position = new Vector2(tile.pos.x, tile.pos.y + 1);
        if (grid.ContainsKey(position))
            neighbours.Add(grid[position]);
        position = new Vector2(tile.pos.x, tile.pos.y -1);
        if (grid.ContainsKey(position))
            neighbours.Add(grid[position]);

        return neighbours;
    }

    public Tile GetElementAt(Vector2 coords)
    {
        if (grid.ContainsKey(coords))
            return grid[coords];
        return null;
    }

    public void RemoveElement(Vector2 coords, bool destroy = true)
    {
        if (destroy)
            Destroy(grid[coords].gameObject);
        grid.Remove(coords);
    }

    public bool CreateElement(Vector2 coords, GameObject obj)
    {
        if (grid.ContainsKey(coords))
            return false;

        GameObject go = Instantiate(obj, coords, Quaternion.identity, transform);

        if(go.TryGetComponent(out Tile tile))
            tile.pos = coords;
        else
            tile = go.AddComponent<Tile>();

        grid.Add(coords, tile);
        return true;
    }

    public bool AddElement(Vector2 coords, GameObject obj)
    {
        if (grid.ContainsKey(coords))
            return false;

        if(obj.TryGetComponent(out Tile tile))
            tile.pos = coords;
        else
            tile = obj.AddComponent<Tile>();

        grid.Add(coords, tile);
        return true;
    }


    public List<Tile> GetBases()
    {
        List<Tile> bases = new List<Tile>();
        foreach (KeyValuePair<Vector2, Tile> t in grid)
        {
            if (t.Value.isBase)
                bases.Add(t.Value);
        }
        return bases;
    }

    /*public List<Tile> GetSpawners()
    {
        List<Tile> bases = new List<Tile>();
        foreach (KeyValuePair<Vector2, Tile> t in grid)
        {
            if (t.Value.isSpawner)
                bases.Add(t.Value);
        }
        return bases;
    }*/


}
