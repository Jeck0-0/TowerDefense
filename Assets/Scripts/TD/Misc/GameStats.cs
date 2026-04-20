using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace TowerDefense
{
    public class GameStats : Singleton<GameStats>
    {
        public int lives;
        public int coins;
        [FormerlySerializedAs("coinsPerSecond")] 
        public int coinsPerTick;
        public event Action livesChanged;
        public event Action coinsChanged;

        private float nextTick;
        private void Update()
        {
            if (!WaveManager.Instance.SpawningPaused)
            {
                if (Time.time > nextTick)
                {
                    ModifyCoins(coinsPerTick);
                    nextTick = Time.time + .5f;
                }
            }
        }

        public void LoseHP(int amount)
        {
            lives -= amount;
            livesChanged?.Invoke();
        }
        
        public void ModifyCoins(int amount)
        {
            coins += amount;
            coinsChanged?.Invoke();
        }

        public void SetCoinsPerSecond(int amount)
        {
            coinsPerTick = amount;
        }
    }
}
