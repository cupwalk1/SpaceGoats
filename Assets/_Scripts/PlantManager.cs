using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class PlantManager
{
   public static List<PlantTile> Plants = new();
   public static void AddPlant(PlantTile plant)
   {
      if (Plants.Contains(plant)) return;
      Plants.Add(plant);
   }
   public static void RefreshTilemaps()
   {

   }
   
}