using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace TowerDefense
{
    public class GameManager : Singleton<GameManager>
    {
        public static int gameTickFrequency = 20;
    
        [SerializeField] GameObject gameOverUI;
        [SerializeField] GameObject victoryUI;
    
        public int winGameAtWave;
    
        public static List<Enemy> Enemies => Instance.enemies;
        public List<Enemy> enemies = new();
        public List<Tower> Towers { get; private set; } = new();

        protected static List<Enemy> newEnemies = new();
        public static void AddEnemy(Enemy enemy)
        {
            if (!Instance.enemies.Contains(enemy) || newEnemies.Contains(enemy))
                newEnemies.Add(enemy);
        }
    
        public static void RemoveEnemy(Enemy enemy)
        {
            if (Instance.enemies.Contains(enemy))
                Instance.enemies.Remove(enemy);
            if (newEnemies.Contains(enemy))
                newEnemies.Remove(enemy);
        }

        private void LateUpdate()
        {
            enemies.AddRange(newEnemies);
            newEnemies.Clear();
        }

        private void Awake()
        {
            GameStats.Instance.livesChanged += CheckGameOver;
            WaveManager.Instance.SpawningNewWave += PauseSpawningOnLastWave;
            WaveManager.Instance.FinishedSpawningWave += CheckVictory;
        }
    
        protected void PauseSpawningOnLastWave(int currentWave)
        {
            if (currentWave + 1 == winGameAtWave)
                WaveManager.Instance.PauseSpawning(false);
        }

        public void AddTower(Tower tower)
        {
            if (Towers.Contains(tower))
            {
                Debug.LogWarning("Tower already registered: " + tower);
                return;
            }
            Towers.Add(tower);
        }

        public void RemoveTower(Tower tower)
        {
            if(!Towers.Contains(tower))
                return;
            Towers.Remove(tower);
        }
        
        public void UnlockGlobalUpgrade(string towerId, IUpgrade upgrade)
        {
            foreach (var tower in Towers)
                if(towerId == "*" || tower.towerID == towerId)
                    tower.upgradeHandler.UnlockUpgrade("globalUpgrade", upgrade, false);
            TowerFactory.Instance.UnlockUpgrade(towerId, upgrade);
        }
        
        protected void CheckVictory()
        {
            if (WaveManager.Instance.CurrentWave + 1 == winGameAtWave)
                CustomCoroutine.Instance.StartWaitOnConditionThenExecute(
                    () => enemies.Count == 0, WinGame);
        }
    
        protected void CheckGameOver()
        {
            if (GameStats.Instance.lives <= 0)
                LoseGame();
        }
    
        protected void LoseGame()
        {
            Time.timeScale = 0;
            gameOverUI.SetActive(true);
            ManagedBehaviour.PauseAll.Add(this);
        }
    
        public void WinGame()
        {
            Time.timeScale = 0;
            victoryUI.SetActive(true);
            ManagedBehaviour.PauseAll.Add(this);
        }
    
        public void ReloadScene()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    
        public void CloseGame()
        {
            Application.Quit();
        }
    
        private void OnDestroy()
        {
            ManagedBehaviour.PauseAll.Remove(this);
        }
    }
}
