using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    static GameStats instance;
    public static event EventHandler OnMoneyChanged;
    public int startMoney = 150;

    public static int Lives;
    public int startLives = 20;

    public static int Rounds;

    static float coins;
    public static float Coins 
    { 
        get { return coins; } 
        set { coins = value; 
              OnMoneyChanged?.Invoke(null,null); } 
    }

    private void Start()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }
        instance = this;

        Coins = startMoney;
        Lives = startLives;
        Rounds = 0;
    }
}
