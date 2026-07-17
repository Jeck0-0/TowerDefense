using System;
using System.Collections.Generic;
using System.Linq;
using GameAnalyticsSDK;
using UnityEngine;

namespace TowerDefense
{
    public class Analytics : Singleton<Analytics>
    {
        private void Awake()
        {
            GameAnalytics.Initialize();
        }

        public void BuildTower(Tower tower)
        {
            GameAnalytics.NewDesignEvent("tower_build", new Dictionary<string, object> {
                { "tower", tower.towerID },
                { "wave", WaveManager.Instance.CurrentWave },
                { "coins", GameStats.Instance.coins },
                { "total", GameManager.Instance.Towers.Count(x=>x.towerID == tower.towerID)}
            });
        }

        public void GameOver()
        {
            GameAnalytics.NewDesignEvent("game_over", new Dictionary<string, object> {
                    { "wave", WaveManager.Instance.CurrentWave },
                    { "coins", GameStats.Instance.coins },
                });
        }

        public void GetTowerUpgrade(ITowerUpgrade upgrade, TowerUpgrades tower)
        {
            GameAnalytics.NewDesignEvent("tower_upgrade", new Dictionary<string, object> {
                { "upgrade", upgrade.CardData.Id },
                { "tower", tower.Tower.towerID },
                { "wave", WaveManager.Instance.CurrentWave },
                { "coins", GameStats.Instance.coins },
            });
        }

        public void TakeDamage(int damage)
        {
            GameAnalytics.NewDesignEvent("take_damage", new Dictionary<string, object> {
                { "remaining_health", GameStats.Instance.lives },
                { "damage_taken", damage },
                { "wave", WaveManager.Instance.CurrentWave },
                { "coins", GameStats.Instance.coins },
            });
        }
    }
}
