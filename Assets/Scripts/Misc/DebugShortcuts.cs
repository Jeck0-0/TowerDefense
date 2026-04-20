using UnityEngine;
using UnityEngine.SceneManagement;


namespace TowerDefense
{
    public class DebugShortcuts : MonoBehaviour
    {
        float timeScaleBeforePausing;
    
        private void Update()
        {
            //toggle fullscreen and save
            if (Input.GetKeyDown(KeyCode.F11))
            {
                bool toggle = !Screen.fullScreen;
                Screen.fullScreen = toggle;
    
                SaveManager.Instance.data.fullscreen = toggle;
                SaveManager.SaveState();
            }
    
            if (Input.GetKeyDown(KeyCode.F1))
                SceneManager.LoadScene("LoadableLevel");
                
            if (Input.GetKeyDown(KeyCode.F2))
                SceneManager.LoadScene("LevelEditor");

            if (Input.GetKeyDown(KeyCode.F3))
                SceneManager.LoadScene("ProceduralLevel");
    
            if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    TowerFactory.Instance.SpawnTower("basic");
                }
                
                //lower volume and save
                if (Input.GetKeyDown(KeyCode.Comma))
                {
                    AudioListener.volume = Mathf.Clamp01(AudioListener.volume - .1f);
                    SaveManager.Instance.data.volume = AudioListener.volume;
                    SaveManager.SaveState();
                    Debug.Log("Volume " + AudioListener.volume);
                }
    
                //increase volume and save
                if (Input.GetKeyDown(KeyCode.Period))
                {
                    AudioListener.volume = Mathf.Clamp01(AudioListener.volume + .1f);
                    SaveManager.Instance.data.volume = AudioListener.volume;
                    SaveManager.SaveState();
                    Debug.Log("Volume " + AudioListener.volume);
                }
    
    
                //Destroy hovering tile
                if (Input.GetKeyDown(KeyCode.D))
                {
                    var t = GridManager.Instance.GetHoveringTile();
                    if (t)
                    {
                        GridManager.Instance.Remove(t);
                        Destroy(t.gameObject);
                        Debug.Log("Destroyed " + t.name);
                    }
                }
    
                //Destroy hovering tower
                if (Input.GetKeyDown(KeyCode.X))
                {
                    Tower t = GridManager.Instance.GetHoveringTile()?.Tower;
                    if (t)
                    {
                        Debug.Log("Destroyed " + t.towerName);
                        t.DestroyTower();
                    }
                }
    
                //Move selected tower
                if (Input.GetKeyDown(KeyCode.V))
                {
                    Tower t = TileSelectionManager.Instance.SelectedTile?.Tower;
                    if (t)
                    {
                        t.StartMoving();
                        TileSelectionManager.Instance?.DeselectTile();
                        Debug.Log("Moving " + t.towerName);
                    }
                }
                
                //+1000 coins
                if (Input.GetKeyDown(KeyCode.M))
                {
                    GameStats.Instance?.ModifyCoins(1000);
                    Debug.Log("+1000 coins");
                }
    
                //Kill enemies
                if (Input.GetKeyDown(KeyCode.K))
                {
                    GameManager.Instance?.enemies.ForEach(x => x.Damage(10000));
                    Debug.Log("Killed all enemies");
                }
    
                //Load main scene
                if (Input.GetKeyDown(KeyCode.W))
                {
                    SceneManager.LoadScene(0);
                    Debug.Log("Loaded main scene");
                }
    
                if (Input.GetKeyDown(KeyCode.G))
                {
                    var g = FindAnyObjectByType<LevelBGIconSpawner>(FindObjectsInactive.Include);
                    if(g) g.gameObject.SetActive(!g.gameObject.activeSelf);
                    Debug.Log("Background effect: " + (g.gameObject.activeSelf ? "on" : "off"));
                }
    
    
    
                //Speed up
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    Time.timeScale *= 2;
                    Debug.Log($"TimeScale: {Time.timeScale}");
                }
                
                //Slow down
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    Time.timeScale /= 2;
                    Debug.Log($"TimeScale: {Time.timeScale}");
                }
    
                //Reset time scale
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    Time.timeScale = 1;
                    Debug.Log($"TimeScale: {Time.timeScale}");
                }
    
                //Toggle pause
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (Time.timeScale > 0f)
                    {
                        timeScaleBeforePausing = Time.timeScale;
                        Time.timeScale = 0;
                        Debug.Log("Paused");
                    }
                    else
                    {
                        Time.timeScale = timeScaleBeforePausing;
                        Debug.Log("Resumed");
                    }
                }
            }
        }
    }
    
}
