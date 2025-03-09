using UnityEngine;


namespace TowerDefense
{
    public class DisableAfterWave : MonoBehaviour
    {
        private void Awake()
        {
            WaveManager.Instance.SpawningNewWave += OnSpawningNewWave;
        }
    
        protected void OnSpawningNewWave(int wave)
        {
            gameObject.SetActive(false);
        }
    
        private void OnDestroy()
        {
            if (WaveManager.Instance)
                WaveManager.Instance.SpawningNewWave -= OnSpawningNewWave;
        }
    }
}
