using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MyEvents : MonoBehaviour
{
    public static MyEvents instance;
    public event EventHandler sium;

    private void Start()
    {
        instance = this;
    }
}
