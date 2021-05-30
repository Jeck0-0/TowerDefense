using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// ///////////////////////////////////////////////////////////////////////////////////////////
/// </summary>
public class GridElement : MonoBehaviour
{
    public Vector2 coords;
    public GridMap map;
    
    

    public GridElement(Vector2 pos)
    {
        coords = pos;
        transform.position = coords;
    }

    public void Remove()
    {
        map.RemoveElement(coords);
    }

}
