using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class SplashWeapon : Weapon
    {
        public override void Awake()
        {
            base.Awake();
            //tower.GetStats().AddModifier("splashDamageArea", "splashWeapon", );
        }
    }
}
