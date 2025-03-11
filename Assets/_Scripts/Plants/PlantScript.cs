using UnityEngine;
using UnityEngine.Tilemaps;

class PlantScript : ResourceScript
{
   public override int MaxTimeToRegen { get; set; } = 10;

   public override bool Harvest()
   {
      return true;
   }
}