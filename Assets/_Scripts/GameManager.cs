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
   public bool IsFreePlay;
   public bool IsGameOver;
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

   public UnityEvent GameStartup = new UnityEvent();
   public UnityEvent GameStart = new UnityEvent();
   public UnityEvent GameLoaded = new UnityEvent();
   public UnityEvent GameEnded = new UnityEvent();
   public UnityEvent MenuLoaded = new UnityEvent();
   public UnityEvent OnPlayerWin = new UnityEvent();
   public UnityEvent OnGameOver = new UnityEvent();
   public UnityEvent OnGameWin = new UnityEvent();

   public UnityEvent ResetGame = new UnityEvent();


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
      OnGameOver.AddListener(delegate { IsGameOver = true; });
      GameEnded.AddListener(EndGame);
      OnPlayerWin.AddListener(delegate { GameManager.Instance.GameEnded.Invoke(); });
      GameStartup.Invoke();
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
   }

   public void QuitGame()
   {
      // Logic to quit the game
      Application.Quit();
   }

   private void OnApplicationQuit()
   {
      SaveGame();
   }

   private void SaveGame()
   {
      SaveGameData();
      ResourceManager.Instance.SaveResources();
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

   public void Victory()
   {
      OnGameWin.Invoke();
      IsGameOver = true;
   }
}


[System.Serializable]
public class GameData
{
   public List<UpgradeBase> Upgrades = new();
}