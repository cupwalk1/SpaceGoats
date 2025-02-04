using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;

public static class PlantManager
{
   private static Task _regenTask;
   private static readonly string SaveFilePath = Path.Combine(Application.persistentDataPath, "plants.json");
   public static Dictionary<Vector3Int, PlantData> Plants = new();

   private static PlantData GetPlant(Vector3Int position)
   {
      if (Plants.TryGetValue(position, out var plantData)) return plantData;
      plantData = new PlantData { Position = position };
      Plants[position] = plantData;
      return plantData;
   }
   
   public static void SavePlants()
   {
      var plantList = Plants.Values.ToList();
      var json = JsonUtility.ToJson(new PlantDataList { Plants = plantList });
      File.WriteAllText(SaveFilePath, json);
   }

   public static void LoadPlants()
   {
      if (!File.Exists(SaveFilePath)) return;
      var json = File.ReadAllText(SaveFilePath);
      var plantList = JsonUtility.FromJson<PlantDataList>(json);
      Plants = plantList.Plants.ToDictionary(p => p.Position);
   }

   public static void StartRegenCounter()
   {
      if (_regenTask != null && !_regenTask.IsCompleted) return;
      _regenTask = RegenCounter();
   }

   private static async Task RegenCounter()
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