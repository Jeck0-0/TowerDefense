using System.Collections.Generic;
using System.Linq;
using TowerDefense;
using UnityEngine;

public class TDGridBackgroundEffect : GridBackgroundEffect
{
    private List<Vector2Int> exclude =  new ();
    
    
    protected override void Start()
    {
        base.Start();
        
        GridManager.Instance.OnTileAdded += OnTileAdded;
        GridManager.Instance.OnTileRemoved += OnTileRemoved;
    
        foreach (var tile in GridManager.Instance.GetAll())
            OnTileAdded(tile);
    }
    private void OnDestroy()
    {
        if(!GridManager.Instance)
            return;
        GridManager.Instance.OnTileAdded -= OnTileAdded;
        GridManager.Instance.OnTileRemoved -= OnTileRemoved;
    }

    protected void OnTileAdded(Tile tile) => SetExcludedTile(tile.Position, true);
    protected void OnTileRemoved(Vector2Int tile) => SetExcludedTile(tile, false);
    

    public void SetExcludedTile(Vector2Int coords, bool excluded)
    {
        if (excluded)
        {
            exclude.Add(coords);
            foreach (var coord in TileToSquares(coords))
                SetExcludedIndex(coord, true);
        }
        else
        {
            exclude.Remove(coords);
            Debug.Log(coords);
            foreach (var coord in TileToSquares(coords))
                SetExcludedIndex(coord, SquareToTiles(coord).Any(x => exclude.Contains(x)));
        }
    }


    public List<Vector2Int> SquareToTiles(int index)
    {
        var list = new List<Vector2Int>();
        Vector2Int first = new Vector2Int(
            index % gridSizeX + (gridSizeY % 2 == 1 ? 0 : gridSizeX / 2) - (gridSizeX + 1) / 2,
            index / gridSizeX - (gridSizeY + 1) / 2);
        list.Add(first);
        list.Add(first + Vector2Int.right);
        list.Add(first + Vector2Int.up);
        list.Add(first + Vector2Int.one);
        Debug.Log(string.Join(" - ", list));
        return list;
    }
    
    
    public List<int> TileToSquares(Vector2Int coords)
    {
        List<int> squares = new List<int>();
        int shift = squareCount / 2 + ((gridSizeY % 2 == 0) ? gridSizeX / 2 : 0);
        squares.Add(coords.x     +  coords.y      * gridSizeX + shift);
        squares.Add(coords.x + 1 +  coords.y      * gridSizeX + shift);
        squares.Add(coords.x     + (coords.y + 1) * gridSizeX + shift);
        squares.Add(coords.x + 1 + (coords.y + 1) * gridSizeX + shift);
        return squares;
    }
}