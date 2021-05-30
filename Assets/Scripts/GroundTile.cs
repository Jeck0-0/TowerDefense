using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile : Tile
{
    [SerializeField] GameObject content;
    [SerializeField] GameObject selectEffect;

    private void Start()
    {
        transform.position = pos;

        enemiesCanWalkOver = false;
        canBuildOver = content == null;
    }

    private void OnMouseEnter()
    {
        //Debug.Log("brrr enter");    
    }

    private void OnMouseDown()
    {
        if (selected)
            Deselect();
        else
            Select();
    }
    private void OnMouseExit()
    {
        //Debug.Log("brrr no mor");
    }

    public void OnDeselect()
    {
        selectEffect.SetActive(false);
    }
    public void OnSelect()
    {
        selectEffect.SetActive(true);
    }

    public void BuildTower(GameObject go)
    {
        canBuildOver = false;
        Instantiate(go, transform);
    }

}
