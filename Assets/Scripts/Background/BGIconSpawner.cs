using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;


namespace TowerDefense
{
    public class BGIconSpawner : MonoBehaviour
    {
        [SerializeField] protected GameObject iconPrefab;
        [SerializeField] protected Vector2 offset;
        [SerializeField] protected List<Vector2Int> ignoreTiles;
        [SerializeField] protected Vector2Int size;
        [SerializeField] protected Dictionary<Vector2Int, BackgroundItem> grid = new();
    
        void Start()
        {
            SpawnTiles();
        }
    
    
        private void Update()
        {
            UpdateTiles();
        }
    
        private void UpdateTiles()
        {
            var cursorPos = Helpers.Camera.ScreenToWorldPoint(Input.mousePosition);
            cursorPos.z = transform.position.z;
    
            foreach (var tile in grid)
            {
                if(tile.Value.gameObject.activeInHierarchy)
                    UpdateTile(tile.Key, tile.Value, cursorPos);
            }
        }
    
        protected virtual void UpdateTile(Vector2Int pos, BackgroundItem item, Vector3 cursorPos)
        {
            var dir = (cursorPos - item.transform.position).normalized;
            var dist = Vector3.Distance(item.transform.position, cursorPos);
    
            item.icon.transform.localScale = Vector3.one * item.scaleCurve.Evaluate(dist);
    
            //icon.transform.rotation = Quaternion.Euler(0, 0, rotationCurve.Evaluate(dist) * 90);
    
            item.icon.transform.localPosition = dir * item.positionCurve.Evaluate(dist);
    
            item.icon.color = item.colorGradient.Evaluate(dist / item.colorMaxDist);
        }
    
        protected virtual void SpawnTiles()
        {
            Vector2Int p;
            for (int x = -size.x / 2; x < size.x / 2; x++)
            {
                for (int y = -size.y / 2; y < size.y / 2; y++)
                {
                    Vector3 spawnPos = new Vector3(x + offset.x, y + offset.y, 0);
                    p = new Vector2Int(x, y);
                    if (!ignoreTiles.Contains(p))
                    {
                        var go = Instantiate(iconPrefab, spawnPos, Quaternion.identity, transform);
                        if (go.TryGetComponent(out BackgroundItem item))
                        {
                            grid.Add(p, item);
                        }
                        else
                        {
                            Debug.LogError("Background item does not have BackgroundItem script", item);
                        }
                    }
                }
            }
    
            //hex grid
            /*for (int x = -size.x / 2; x < size.x / 2; x++) 
            {
                for (int y = -size.y / 2; y < size.y / 2; y++)
                {
                    float offset = y % 2 == 0 ? 0 : (2f/3f*distance);
                    Vector3 spawnPos = new Vector3(x * Mathf.Sqrt(3) * (distance * 2 / 3) + offset, y * (3 / 2) * distance, 0);
                    p = new Vector2Int(x, y);
                    if (!ignoreTiles.Contains(p))
                        Instantiate(iconPrefab, spawnPos, Quaternion.identity);
                }
            }*/
        }
    }
    
}
