#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Scripts;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Developer
{
   // Add a Developer menu at the top of the Unity editor
   [MenuItem("Developer/Respawn Player")]
   private static void Respawn()
   {
      GameManager.Instance.RestartGame();
   }

   [MenuItem("Tools/Replace Circle Image")]
   public static void ReplaceImages()
   {
      Sprite newSprite = Selection.activeObject as Sprite;
      if (newSprite == null)
      {
         Debug.LogWarning("Select a sprite in the Project window before running this tool.");
         return;
      }

      Image[] images = Object.FindObjectsOfType<Image>(true);
      int replacedCount = 0;

      foreach (Image img in images)
      {
         if (img.sprite != null && img.sprite.name is "Circle" or "Circle_8px" or "Circle_16px" or "9-Sliced" or "Background")
         {
            Undo.RecordObject(img, "Replace Circle Image");
            img.sprite = newSprite;
            replacedCount++;
         }
      }

      Debug.Log($"Replaced {replacedCount} images.");
   }
   
   
   [MenuItem("Tools/Select Circle Images")]
   public static void SelectImages()
   {
      List<GameObject> selectedObjects = new List<GameObject>();
      Image[] images = Object.FindObjectsOfType<Image>(true);
        
      foreach (Image img in images)
      {
         if (img.sprite != null && img.sprite.name is "Circle" or "9-Sliced" or "Background" or "Circle_8px" or "Circle_16px")
         {
            selectedObjects.Add(img.gameObject);
         }
      }
        
      if (selectedObjects.Count > 0)
      {
         Selection.objects = selectedObjects.ToArray();
         Debug.Log($"Selected {selectedObjects.Count} images.");
      }
      else
      {
         Debug.Log("No matching images found.");
      }
   }

   // Erase plants from plants.json
   [MenuItem("Developer/Erase Saves")]
   private static void ErasePlants()
   {
      ResourceManager.Instance.Resources.Clear();
      ResourceManager.Instance.SaveResources();

      File.Create(Application.persistentDataPath + "/gameData.json");
   }


   // Button to start game through _pm.GameStart.Invoke()
   [MenuItem("Developer/Start Game")]
   private static void StartGame()
   {
      GameManager.Instance.GameStart.Invoke();
   }

   [MenuItem("Developer/Refresh Resource Tiles")]
   private static void method1()
   {
      Tilemap tilemap = GameObject.Find("Ground & Resouces").GetComponent<Tilemap>();
      tilemap.RefreshAllTiles();
   }

   [MenuItem("Developer/GetHarvestableResources/GameEnergy")]
   private static void method2()
   {
      Debug.Log(ResourceManager.Instance.GetHarvestableResources("GameEnergy"));
   }

   [MenuItem("Developer/GetHarvestableResources/GamePlants")]
   private static void method3()
   {
      Debug.Log(ResourceManager.Instance.GetHarvestableResources("GamePlants"));
   }

   //GameMaterials
   [MenuItem("Developer/GetHarvestableResources/GameMaterials")]
   private static void method4()
   {
      Debug.Log(ResourceManager.Instance.GetHarvestableResources("GameMaterial"));
   }


   [MenuItem("Developer/Upgrades/PurchaseExtraLife")]
   private static void UnlockExtraLife()
   {
      UpgradeManager.Instance.AddUpgrade(new ExtraLifeUpgrade());
   }

   [MenuItem("Developer/Upgrades/PurchaseIncreaseOxygenCapacity")]
   private static void UnlockIncreaseOxygenCapacity()
   {
      UpgradeManager.Instance.AddUpgrade(new IncreaseOxygenCapacityUpgrade());
   }

   [MenuItem("Developer/Upgrades/PurchaseIncreasePlantCapacity")]
   private static void UnlockIncreasePlantCapacity()
   {
      UpgradeManager.Instance.AddUpgrade(new IncreasePlantCapacityUpgrade());
   }
   
   [MenuItem("Tools/Create Prefab from Selection")]
   public static void CreatePrefab()
   {
      if (Selection.gameObjects.Length == 0)
      {
         Debug.LogWarning("No GameObjects selected to create a prefab.");
         return;
      }
        
      string path = "Assets/Prefabs/GeneratedPrefab.prefab";
      GameObject firstObject = Selection.gameObjects[0];
      GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(firstObject, path, InteractionMode.UserAction);
        
      if (prefab == null)
      {
         Debug.LogError("Failed to create prefab.");
         return;
      }
        
      for (int i = 1; i < Selection.gameObjects.Length; i++)
      {
         PrefabUtility.ApplyObjectOverride(Selection.gameObjects[i], path, InteractionMode.UserAction);
      }
        
      Debug.Log($"Prefab created at {path}, and instances linked without losing modifications.");
   }

   // public class IncreaseOxygenCapacityUpgrade : UpgradeBase
   // {
   //    public IncreaseOxygenCapacityUpgrade()
   //    {
   //       Price = 100;
   //       Name = "Aumento Capacità";
   //       Description = "Aumenta la capacità di ossigeno di 10 secondi";
   //    }
   //    public override void OnEnable()
   //    {
   //       GameObject.FindWithTag("Player").GetComponent<PlayerManager>().MaxOxygen += 10;
   //    }
   // }
   //
   // public class IncreasePlantCapacityUpgrade : UpgradeBase
   // {
   //    public IncreasePlantCapacityUpgrade()
   //    {
   //       Price = 100;
   //       Name = "Aumento Capacità";
   //       Description = "Aumenta la capacità di piante di 1";
   //    }
   //    public override void OnEnable()
   //    {
   //       GameObject.FindWithTag("Player").GetComponent<PlayerManager>().MaxPlants++;
   //    }
   // }
}


#endif