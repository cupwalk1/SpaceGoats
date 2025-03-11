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
   public static ResourceManager Instance { get; private set; }
   private static Task _regenTask;
   private string SaveFilePath;
   public Dictionary<Vector3Int, ResourceData> Resouces = new();
   public UnityEvent<int> _onPlantGathered = new UnityEvent<int>();
   public DateTime lastSave;
   
   int maxCapacity = 10;
   

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
      SaveFilePath = Path.Combine(Application.persistentDataPath, "resources.json");
      GameManager.Instance.GameLoaded.AddListener(RefreshTiles);
      GameManager.Instance.GameLoaded.AddListener(LoadResources);
      
   }


   public void RefreshTiles()
   {
      GameObject.FindGameObjectWithTag("tilemap").GetComponent<Tilemap>().RefreshAllTiles();
   }
   
   public void SaveResources()
   {
      var plantList = Resouces.Values.ToList();
      var json = JsonUtility.ToJson(new ResourceDataList(plantList, DateTime.Now));
      File.WriteAllText(SaveFilePath, json);
   }
   
   public void LoadResources()
   {
      if (!File.Exists(SaveFilePath)) return;
      var json = File.ReadAllText(SaveFilePath);
      var plantList = JsonUtility.FromJson<ResourceDataList>(json);
      Resouces = plantList.ResourceList.ToDictionary(p => p.Position);
      lastSave = plantList.GetSaveTime(); // Use GetSaveTime to convert string back to DateTime
   }
   

   
}

[Serializable]
public class ResourceDataList
{
   public ResourceDataList(List<ResourceData> r, DateTime t)
   {
      ResourceList = r;
      saveTime = t.ToString("o"); // Convert DateTime to ISO 8601 string
   }
   public string saveTime; // Change DateTime to string
   public List<ResourceData> ResourceList;

   public DateTime GetSaveTime()
   {
      return DateTime.Parse(saveTime); // Convert string back to DateTime
   }
}