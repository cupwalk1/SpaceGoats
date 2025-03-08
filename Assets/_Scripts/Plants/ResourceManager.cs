using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class ResourceManager : MonoBehaviour
{
   [SerializeField] private TMP_Text text;
   public static ResourceManager Instance { get; private set; }
   private static Task _regenTask;
   private string SaveFilePath;
   public Dictionary<Vector3Int, ResourceData> Resouces = new();
   public UnityEvent<int> _onPlantGathered = new UnityEvent<int>();
   public DateTime lastSave;
   
   int maxCapacity = 10;

   public int PlantsGatheredDuringRun
   {
      get => GameManager.Instance.gameData.PlantsGatheredDuringRun;
      set => GameManager.Instance.gameData.PlantsGatheredDuringRun = value;
   }

   public ResourceData GetResource(Vector3Int position)
   {
      if (Resouces.TryGetValue(position, out var plantData))
         return plantData;
      Resouces[position] = new ResourceData { Position = position};
      return Resouces[position];
   }
   
   
   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
         DontDestroyOnLoad(gameObject);
         LoadResources();
      }
      else
      {
         Destroy(gameObject);
      }
   }

   private void Start()
   {
      _onPlantGathered.AddListener(delegate(int i) { text.text = i.ToString(); });
      SaveFilePath = Path.Combine(Application.persistentDataPath, "resources.json");
      PlantsGatheredDuringRun = 0;
      text.text = "0";
   }


   public void OnPlantGathered()
   {
      _onPlantGathered.Invoke(PlantsGatheredDuringRun);
   }
   
   public void SaveResources()
   {
      var plantList = Resouces.Values.ToList();
      var json = JsonUtility.ToJson(new ResourceDataList { ResourceList = plantList, saveTime = DateTime.Now});
      File.WriteAllText(SaveFilePath, json);
   }
   
   public void LoadResources()
   {
      if (!File.Exists(SaveFilePath)) return;
      var json = File.ReadAllText(SaveFilePath);
      var plantList = JsonUtility.FromJson<ResourceDataList>(json);
      Resouces = plantList.ResourceList.ToDictionary(p => p.Position);
      lastSave = plantList.saveTime;
   }
   

   
}

[Serializable]
public class ResourceDataList
{
   public DateTime saveTime;
   [FormerlySerializedAs("Plants")] public List<ResourceData> ResourceList;
}