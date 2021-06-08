using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    //public Projectile projectile; //UPDATE SUBSCRIPTS WHEN UPDATING THESE
    public float power;
    public string turretName;
    public string description;
    public int cost;
    public int sellRefund;
    int timesShot;
    int enemiesKilled;
    float damageDealt;
    float moneySpentOnTurret;
    
    private void Start()
    {
        Shooting[] shooting = GetComponents<Shooting>();
        foreach(Shooting s in shooting)
            s.ShotProjectile += OnShotProjectile;

    }

    void OnShotProjectile(object sender, Projectile proj)
    {
        proj.OnHit += OnProjectileHit;
    }

    void OnProjectileHit(object sender, Health.DamageArgs args)
    {
        damageDealt += args.damageDealt;
        if (args.killed)
            enemiesKilled++;
    }

    public void Sell()
    {
        GameStats.Coins += sellRefund;
        Destroy(gameObject);
    }

    public void Upgrade()
    {

    }


}

// single static event with all enemy death informations handled by single script
// many scripts siumming
// idk