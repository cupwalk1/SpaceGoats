using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class PlantManager : MonoBehaviour
{
   [SerializeField] private TMP_Text text;
   public static PlantManager Instance { get; private set; }
   private static Task _regenTask;
   private string SaveFilePath;
   public Dictionary<Vector3Int, PlantData> Plants = new();
   public UnityEvent<int> _onPlantGathered = new UnityEvent<int>();


   public int PlantsGatheredDuringRun
   {
      get => GameManager.Instance.gameData.PlantsGatheredDuringRun;
      set => GameManager.Instance.gameData.PlantsGatheredDuringRun = value;
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
   }

   private void Start()
   {
      _onPlantGathered.AddListener(delegate(int i) { text.text = i.ToString(); });
      SaveFilePath = Path.Combine(Application.persistentDataPath, "plants.json");
      LoadPlants();
      StartRegenCounter();
      PlantsGatheredDuringRun = 0;
      text.text = "0";
   }


   public void OnPlantGathered()
   {
      _onPlantGathered.Invoke(PlantsGatheredDuringRun);
   }

   public void SavePlants()
   {
      var plantList = Plants.Values.ToList();
      var json = JsonUtility.ToJson(new PlantDataList { Plants = plantList });
      File.WriteAllText(SaveFilePath, json);
   }

   public void LoadPlants()
   {
      if (!File.Exists(SaveFilePath)) return;
      var json = File.ReadAllText(SaveFilePath);
      var plantList = JsonUtility.FromJson<PlantDataList>(json);
      Plants = plantList.Plants.ToDictionary(p => p.Position);
   }

   public void StartRegenCounter()
   {
      if (_regenTask != null && !_regenTask.IsCompleted) return;
      _regenTask = RegenCounter();
   }

   private async Task RegenCounter()
   {
      while (true)
      {
         foreach (var plant in Plants)
         {
            if (plant.Value.IsRipe) continue;
            plant.Value.TimeToRipe--;
            if (plant.Value.TimeToRipe <= 0)
            {
               plant.Value.IsRipe = true;
               plant.Value.TimeToRipe = 0;
               GameObject.Find("Plants").GetComponent<Tilemap>().RefreshTile(plant.Key);
            }
         }

         await Task.Delay(1000);
      }
   }
}

[System.Serializable]
public class PlantDataList
{
   public List<PlantData> Plants;
}