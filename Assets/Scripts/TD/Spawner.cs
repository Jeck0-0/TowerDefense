using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TowerDefense
{
    public class Spawner : MonoBehaviour
    {
        public int startSpawningAfterWaves = 0;
        public List<Wave> waves;
    
        public bool isSpawning;
    
        protected int wavesWhileActive;
        protected int currentWaveIndex;
    
        public float multiplyHpPerWave = 1.1f;
        public float multiplyHpBase = 1f;
    
        public float SpawnNextWave()
        {
            if (!isActiveAndEnabled)
                return -1;
    
            wavesWhileActive++;
            if (wavesWhileActive < startSpawningAfterWaves)
                return -1;
    
            if (currentWaveIndex >= waves.Count)
                return -1;
    
            StartCoroutine(SpawnWaveCoroutine());
            float delayAfter = waves[currentWaveIndex].advancedSettings.customDelayAfterWave;
    
            currentWaveIndex++;
            if(currentWaveIndex >= waves.Count)
                currentWaveIndex = 0;
    
            return delayAfter;
        }
    
        protected IEnumerator SpawnWaveCoroutine()
        {
            isSpawning = true;
            Wave wave = waves[currentWaveIndex];
            for (int i = 0; i < wave.amount; i++)
            {
                foreach (Wave.WaveEnemy e in wave.enemies)
                {
                    var enemy = Instantiate(e.prefab, transform.position, Quaternion.identity).GetComponent<Enemy>();
                        
                    enemy.Initialize();
                    if (wave.advancedSettings.hpMultiplier != 1)
                        enemy.stats.AddModifier("maxHealth", "waveCustomHealth", multiply: wave.advancedSettings.hpMultiplier);
                        
                    float waveScaling = 1 + (WaveManager.Instance.CurrentWave * (multiplyHpPerWave - 1));
                    enemy.stats.AddModifier("maxHealth", "waveScaling", multiply: waveScaling * multiplyHpBase);
    
                    yield return Helpers.GetWait(e.delay);
                }
            }
            isSpawning = false;
        }
    }
    
    
    
    [Serializable]
    public class Wave
    {
        public AdvancedSettings advancedSettings;
        [TableList(ShowIndexLabels = true)]
        public List<WaveEnemy> enemies;
        public int amount = 5;
    
        [Serializable]
        public class WaveEnemy
        {
            [TableColumnWidth(50, Resizable = false)]
            [PreviewField(Alignment = ObjectFieldAlignment.Center)]
            public GameObject prefab;
            [VerticalGroup("Settings"), LabelWidth(50)]
            public float delay = .5f;
        }
    
        [Serializable]
        public class AdvancedSettings
        {
            public float customDelayAfterWave = -1;
            public float hpMultiplier = 1;
        }
    }
}
