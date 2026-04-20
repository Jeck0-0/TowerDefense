using System.IO;
using System;
using UnityEngine;


namespace TowerDefense
{
    public class SaveManager : Singleton<SaveManager>
    {
        public SaveData data;
        const string SAVE_FILE_NAME = "SaveData.txt";
        public static event Action OnApplySettings; 
        
        private void Awake()
        {
            if(IsInstanced) 
            {
                Destroy(gameObject);
                return;
            }
    
            LoadState();
            DontDestroyOnLoad(gameObject);
        }
    
        public static void SaveState()
        {
            string saveJson = JsonUtility.ToJson(Instance.data);
            string savePath = Path.Combine(Application.dataPath, SAVE_FILE_NAME);
            File.WriteAllText(savePath, saveJson);
        }
    
        public static void LoadState() 
        {
            string savePath = Path.Combine(Application.dataPath, SAVE_FILE_NAME);
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                Instance.data = JsonUtility.FromJson<SaveData>(json);
            }
            else
            {
                Instance.data = new SaveData();
                SaveState();
            }
            
            Instance.ApplySettings();
            OnApplySettings?.Invoke();
        }
    
        void ApplySettings()
        {
            Screen.fullScreen = data.fullscreen;
        }
    }
    
}
