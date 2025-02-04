using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class PlantManager
{
   private static readonly string SaveFilePath = Path.Combine(Application.persistentDataPath, "plants.json");
   public static Dictionary<Vector3Int, PlantData> Plants = new();

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
}

[System.Serializable]
public class PlantDataList
{
   public List<PlantData> Plants;
}