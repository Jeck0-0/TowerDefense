using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    public Dictionary<Vector2, GridElement> grid;

    public GridElement GetElementAt(Vector2 coords)
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

    public bool AddElement(Vector2 coords, GameObject obj)
    {
        if (grid.ContainsKey(coords))
            return false;
        GameObject go = Instantiate(obj, coords, Quaternion.identity, transform);
        GridElement ge = go.AddComponent<GridElement>();
        grid.Add(coords, ge);
        return true;
    }

    

}