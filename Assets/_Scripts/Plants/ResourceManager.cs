using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ResourceManager : MonoBehaviour
{
   public static ResourceManager Instance;
   private static Task _regenTask;
   public ResourceInfo ResourceInfo;
   public ResourceInfo DefaultResourceInfo;
   public GoatStats GoatStats;
   public GoatStats DefaultGoatStats;
   
   private int _totalMaterials;


   private string SaveFilePath =>
      Path.Combine(Application.persistentDataPath, "resources.json");

   public int RandomMaxEnergyTime => Random.Range(ResourceInfo.averageGeneratorBreakingTime - ResourceInfo.semiRangeGeneratorBreakingTime,
      ResourceInfo.averageGeneratorBreakingTime + ResourceInfo.semiRangeGeneratorBreakingTime);



private Coroutine miningCoroutine;
   public Coroutine MineOreCoroutine
   {
      set
      {
         if (miningCoroutine != null) return;
         miningCoroutine = value;
      }
      get
      {
         return miningCoroutine;
      }
   }
   
   
   public void StartMiner()
   {
      MineOreCoroutine = StartCoroutine(MineOre());
   }
   
   
   
   
   
   
   
   public List<ResourceData> Resources = new();
   public Dictionary<string, int> ResourceCountInScene = new();
   public UnityEvent<int> _onPlantGathered = new UnityEvent<int>();
   public UnityEvent OnResourcesChanged = new UnityEvent();

   [Header("Resources")] private int _totalFood;

   public int TotalFood
   {
      get => _totalFood;
      set
      {
         OnResourcesChanged.Invoke();
         _totalFood = Mathf.Clamp(value, 0, ResourceInfo.maxFruitsInWarehouse);
      }
   }


   public int TotalEnergy
   {
      get
      {
         var c = Resources.Where(s => s.Type == ResourceData.ResourceType.Energy).ToList();
         float r = (float)c.Count(s => !s.IsRipe) / c.Count();
         return (int)(r*ResourceInfo.energyAvailable);
      }
   }

   public int TotalMaterials
   {
      get => _totalMaterials;
      set
      {
         OnResourcesChanged.Invoke();
         _totalMaterials = value;
      }
   }

   [Header("Resources Gathered During Game")]
   public int PlantsGathered;

   public int MaterialsGathered;


   public int GetHarvestableResources(string SceneName)
   {
      var resList = JsonUtility.FromJson<ResourceDataList>(File.ReadAllText(SaveFilePath)).ResourceList;
      return resList.Count(s => s.IsRipe);
   }


   public ResourceData GetResource(Vector3 position, ResourceData.ResourceType type, string sceneName = null)
   {
      if (sceneName == null)
         sceneName = SceneManager.GetActiveScene().name;
      var r = Resources.FirstOrDefault(p => p.Position == position && p.SceneName == sceneName && p.Type == type);
      if (r != null)
         return r;
      r = new ResourceData(type, sceneName, position, type == ResourceData.ResourceType.Energy
         ? RandomMaxEnergyTime
         : 0);
      Resources.Add(r);
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
      GameManager.Instance.OnPlayerWin.AddListener(GainResources);
      TotalMaterials = PlayerPrefs.GetInt("TotalMaterials", 0);
      TotalFood = PlayerPrefs.GetInt("TotalFood", ResourceInfo.maxFruitsInWarehouse / 2);
      GameManager.Instance.GameLoaded.AddListener(ResetGatheredResources);
      StartCoroutine(Eating());
      StartCoroutine(DeductTime());
   }

   private void ResetGatheredResources()
   {
      MaterialsGathered = 0;
      PlantsGathered = 0;
   }

   public bool HasEnoughResources(ResourceCost cost)
   {
      // Make sure we have enough of each
      return (TotalMaterials >= cost.materials) &&
             (TotalFood >= cost.fruits) &&
             (TotalEnergy >= cost.power);
   }

   public void DeductResources(ResourceCost cost)
   {
      TotalMaterials -= cost.materials;
      TotalFood -= cost.fruits;
   }


   private void GainResources()
   {
      TotalFood += PlantsGathered;
      TotalMaterials += MaterialsGathered;
   }

   public void SaveResources()
   {
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
         SceneManager.LoadScene("GameEnergy");
         return;
      }

      var json = File.ReadAllText(SaveFilePath);
      var plantList = JsonUtility.FromJson<ResourceDataList>(json);
      Resources = plantList.ResourceList;
      PlantsGathered = 0;
      MaterialsGathered = 0;
   }

   IEnumerator DeductTime()
   {
      while (gameObject)
      {
         yield return new WaitForSeconds(1);
         foreach (var s in Resources)
         {
            if (s.TimeToRipe > 0)
            {
               s.TimeToRipe--;
               if (s.TimeToRipe == 0)
               {
                  OnResourcesChanged.Invoke();
               }
            }

            if (s.ResourceGameObject)
            {
               s.ResourceGameObject.GetComponent<Animator>().SetInteger("TimeToRipe", (int)s.TimeToRipe);
               s.ResourceGameObject.GetComponent<ResourceScript>().minimapSprite.color = s.IsRipe
                  ? s.ResourceGameObject.GetComponent<ResourceScript>().readyColor
                  : s.ResourceGameObject.GetComponent<ResourceScript>().notReadyColor;
            }
         }

      }
   }

   IEnumerator Eating()
      {
         while (gameObject)
         {
            yield return new WaitForSeconds(ResourceInfo.fruitEatingRate);
            TotalFood--;
         }
      }

   IEnumerator MineOre()
   {
      while (gameObject)
      {
         yield return new WaitForSeconds(ResourceInfo.oreMiningRate);
         TotalMaterials++;
      }
   }

      public (ResourceData.ResourceType type, string levelName) GetResourceType(Vector3 position)
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