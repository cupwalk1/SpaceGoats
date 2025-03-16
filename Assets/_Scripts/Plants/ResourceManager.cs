using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class ResourceManager : MonoBehaviour
{
   public static ResourceManager Instance;
   private static Task _regenTask;

   private string SaveFilePath =>
      Path.Combine(Application.persistentDataPath, "resources.json");

   public List<ResourceData> Resources = new();
   public Dictionary<string, int> ResourceCountInScene = new();
   public UnityEvent<int> _onPlantGathered = new UnityEvent<int>();

   public int EnergyProducedByEachGenerator;

   [Header("Resources")] public int TotalFood;
   public int TotalMaterials;

   public int TotalEnergy
   {

      get
      {
         var c = JsonUtility
            .FromJson<ResourceDataList>(File.ReadAllText(SaveFilePath));
         return c.ResourceList.Count(s => s.TimeToRipe >= (DateTime.Now - s.GetSaveTime()).TotalSeconds && s.Type == ResourceData.ResourceType.Energy);
      }
   } 

   private int EnergyCapacity;


   [Header("Resources Gathered During Game")]
   public int PlantsGathered;

   public int MaterialsGathered;

   public int PlantMaxCapacity = 20;


   public int GetHarvestableResources(string SceneName)
   {
      var resList = JsonUtility.FromJson<ResourceDataList>(File.ReadAllText(SaveFilePath)).ResourceList;
      return resList.Count(s => s.TimeToRipe <= (DateTime.Now - s.GetSaveTime()).TotalSeconds);
   }


   public ResourceData GetResource(Vector3Int position, ResourceData.ResourceType type, string sceneName = null)
   {
      if (sceneName == null)
         sceneName = SceneManager.GetActiveScene().name;
      var r = Resources.FirstOrDefault(p => p.Position == position && p.SceneName == sceneName && p.Type == type);
      if (r != null)
         return r;
      r = new ResourceData(type, sceneName, position, 0);
      Resources.Add(r);
      Debug.Log("Created new resource");
      return r;
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
      GameManager.Instance.GameLoaded.AddListener(RefreshTiles);
      GameManager.Instance.GameLoaded.AddListener(LoadResources);
      GameManager.Instance.OnPlayerWin.AddListener(GainResources);

      TotalMaterials = PlayerPrefs.GetInt("TotalMaterials", 0);
      TotalFood = PlayerPrefs.GetInt("TotalFood", 0);
   }

   private void GainResources()
   {
      TotalFood += PlantsGathered;
      TotalMaterials += MaterialsGathered;
   }
   
   public void RefreshTiles()
   {
      GameObject.FindGameObjectWithTag("tilemap").GetComponent<Tilemap>().RefreshAllTiles();
   }

   public void SaveResources()
   {
      foreach (var r in Resources.Where(s => s.SceneName == SceneManager.GetActiveScene().name))
      {
         r.SetSaveTime(DateTime.Now);
      }
      var json = JsonUtility.ToJson(new ResourceDataList(Resources));
      File.WriteAllText(SaveFilePath, json);

      PlayerPrefs.SetInt("TotalFood", TotalFood);
      PlayerPrefs.SetInt("TotalMaterials", TotalMaterials);
   }

   public void LoadResources()
   {
      if (!File.Exists(SaveFilePath))
      {
         var j = JsonUtility.ToJson(new ResourceDataList(Resources));
         File.WriteAllText(SaveFilePath, j);
         Debug.Log("File not found");
         return;
      }
      var json = File.ReadAllText(SaveFilePath);
      var plantList = JsonUtility.FromJson<ResourceDataList>(json);
      Resources = plantList.ResourceList;
      PlantsGathered = 0;
      MaterialsGathered = 0;
   }

   public (ResourceData.ResourceType type, string levelName) GetResourceType(Vector3Int position)
   {
      var t = Resources.FirstOrDefault(p =>
         p.Position == position && p.SceneName == SceneManager.GetActiveScene().name);
      if (t != null)
         return (t.Type, t.SceneName);
      Debug.Log("Resource not found");
      return (ResourceData.ResourceType.None, SceneManager.GetActiveScene().name);
   }
}

[Serializable]
public class ResourceDataList
{
   public ResourceDataList(List<ResourceData> r)
   {
      ResourceList = r;
   }
   public List<ResourceData> ResourceList;
}