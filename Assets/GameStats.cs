using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static int
        Money;
    public int startMoney = 150;

    public static int Lives;
    public int startLives = 20;

    public static int Rounds;

    private void Start()
    {
        Money = startMoney;
        Lives = startLives;
        Rounds = 0;
    }
}
