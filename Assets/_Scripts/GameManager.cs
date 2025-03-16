using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using _Scripts;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public bool IsInGameScene => SceneManager.GetActiveScene().buildIndex != 0;
    
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
    public UnityEvent GameEnded = new UnityEvent();
    public UnityEvent MenuLoaded = new UnityEvent();
    public UnityEvent OnPlayerWin = new UnityEvent();
    
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
    }

    private void Start()
    {
        GameEnded.AddListener(EndGame);
        OnPlayerWin.AddListener(delegate{GameManager.Instance.GameEnded.Invoke();});
    }

    public void StartGame()
    {
        GameStart.Invoke();
    }

    public void EndGame()
    {
        SaveGameData();
        SceneManager.LoadScene("MenuScene");
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
      if(IsInGameScene)
      {
          ResourceManager.Instance.SaveResources();
      }
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
        if (IsInGameScene)
        {
            GameLoaded.Invoke();
            Invoke("StartGame", 2);
        }
        else
        {
            MenuLoaded.Invoke();
        }
    }
}


[System.Serializable]
public class GameData
{
    public List<UpgradeBase> Upgrades = new();
}

