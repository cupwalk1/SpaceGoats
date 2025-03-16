#if UNITY_EDITOR
using System.IO;
using _Scripts;
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