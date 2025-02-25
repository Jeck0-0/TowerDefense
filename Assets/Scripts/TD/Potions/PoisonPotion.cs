using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonPotion : Potion
{
    public float duration = 5;
    public float dps = 20;

    public override void PlayPotion(Vector2 position)
    {
        foreach (Enemy e in GameManager.Enemies)
        {
            if (Vector2.Distance(e.transform.position, position) > range)
                continue;

            PoisonEffect poisonEffect = new PoisonEffect(e, dps);
            e.EffectHandler.AddEffect("poison", poisonEffect, duration);
        }
    }
}