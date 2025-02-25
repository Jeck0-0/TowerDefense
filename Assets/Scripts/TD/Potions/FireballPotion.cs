using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballPotion : Potion 
{
    public float damage = 80;


    public override void PlayPotion(Vector2 position)
    {
        foreach(Enemy e in GameManager.Enemies)
        {
            if (Vector2.Distance(e.transform.position, position) > range)
                continue;

            e.Damage(damage);
        }
    }
}