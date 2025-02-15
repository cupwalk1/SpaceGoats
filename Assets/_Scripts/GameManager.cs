using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using _Scripts;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private GameData _gameData;
    
    public GameData gameData
    {
        set { _gameData = value; }
        get
        {
            if (_gameData == null)
            {
                _gameData = LoadGameData();
            }
            return _gameData;
        }
    }
    
    public UnityEvent GameStart = new UnityEvent();
    public UnityEvent GameLoaded = new UnityEvent();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
        GameObject.FindWithTag("Player").GetComponent<PlayerManager>().OnPlayerDie.AddListener(EndGame);
    }

    public void StartGame()
    {
        // Logic to start the game
        SceneManager.LoadScene("GameScene");
    }

    public void EndGame()
    {
        SaveGameData();
    }

    public void RestartGame()
    {
        GameLoaded.Invoke();
        GameStart.Invoke();
    }

    public void QuitGame()
    {
        // Logic to quit the game
        Application.Quit();
    }
    
    private void OnApplicationQuit()
    {
        Debug.Log("Game quit");
        SaveGame();
    }

    private void SaveGame()
    {
      SaveGameData();
      PlantManager.Instance.SavePlants();
    }
   
    public void SaveGameData()
    {
        var json = JsonUtility.ToJson(_gameData);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "gameData.json"), json);
    }
   
    static GameData LoadGameData()
    {
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "gameData.json")))
        {
            File.Create(Path.Combine(Application.persistentDataPath, "gameData.json"));
            return new GameData();
        }
        var json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "gameData.json"));
        return JsonUtility.FromJson<GameData>(json);
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameLoaded.Invoke();
    }
}

[System.Serializable]
public class GameData
{
    public int PlantsGatheredDuringRun;

    public List<UpgradeBase> Upgrades = new();
}

