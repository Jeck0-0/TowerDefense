using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static List<Enemy> enemies = new List<Enemy>(); /* { 
        get { return enemies; } 
        set { Debug.Log("sium"); } 
    }*/

    public Health health;
    public float distanceFromTarget;

    private void Start()
    {
        enemies.Add(this);
        if (gameObject.TryGetComponent(out health))
            health.OnDied += Die;
        else
            Debug.LogError("Enemy has no health script", gameObject);
    }

    public void Die(object sender, EventArgs e)
    {
        // effect
        enemies.Remove(this);
        Destroy(gameObject);
    }

}
