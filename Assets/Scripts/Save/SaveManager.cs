using System.IO;
using UnityEngine;


namespace TowerDefense
{
    public class SaveManager : Singleton<SaveManager>
    {
        public SaveData data;
        const string SAVE_FILE_NAME = "SaveData.txt";
    
        private void Awake()
        {
            if(isInstanced) 
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
        }
    
        void ApplySettings()
        {
            AudioListener.volume = data.volume;
            Screen.fullScreen = data.fullscreen;
        }
    }
    
}
