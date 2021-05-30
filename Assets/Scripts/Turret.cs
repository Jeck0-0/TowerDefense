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
    int timesShot;
    int enemiesKilled;
    float damageDealt;

    void Sell()
    {

    }

    void Upgrade()
    {

    }


}

// single static event with all enemy death informations handled by single script
// many scripts siumming
// idk