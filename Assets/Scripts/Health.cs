using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;

    public event EventHandler OnDied;
    public event EventHandler OnDamaged;

    public delegate void Sium();

    //bool dead = false;

    public void ModifyHealth(float amount)
    {
        if (amount <= 0)
            Damage(amount);
        else
            Heal(amount);
    }

    public DamageArgs Damage(float amount) // returns true if killed
    {
        if (amount <= 0)
            return null;

        float healthBefore = currentHealth;
        currentHealth -= amount;
        OnDamaged?.Invoke(this, null);
        if (currentHealth <= 0)
        {
            OnDied?.Invoke(this, null);
            return new DamageArgs(healthBefore, true);
        }
        return new DamageArgs(amount, false);
    }

    public bool Heal(float amount) // returns true if health is full
    {
        if (amount <= 0)
            return currentHealth >= maxHealth;

        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        return currentHealth == maxHealth;
    }

    public class DamageArgs
    {
        public float damageDealt;
        public bool killed;
        public DamageArgs(float _damageDealt, bool _killed = false)
        {
            damageDealt = _damageDealt;
            killed = _killed;
        }
    }
}
