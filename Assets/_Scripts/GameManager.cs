using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
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
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        // Logic to restart the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
      PlantManager.Instance.SavePlants();
    }
   
    static void SaveGameData()
    {
        var gameData = new GameData { PlantsGatheredDuringRun = PlantManager.Instance.PlantsGatheredDuringRun };
        var json = JsonUtility.ToJson(gameData);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "gameData.json"), json);
    }
   
    static GameData LoadGameData()
    {
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "gameData.json"))) return new GameData();
        var json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "gameData.json"));
        return JsonUtility.FromJson<GameData>(json);
    }
    
    public UnityEvent GameStart = new UnityEvent();
    
}

[System.Serializable]
public class GameData
{
    public int PlantsGatheredDuringRun;
}

