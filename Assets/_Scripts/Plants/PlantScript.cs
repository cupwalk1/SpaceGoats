using UnityEngine;
using UnityEngine.Tilemaps;

class PlantScript : ResourceScript
{
   public override int MaxTimeToRegen => resourceInfo.plantRegenTime;

   public override ResourceData.ResourceType Type => ResourceData.ResourceType.Plant;
   public override bool Harvest()
   {
      _RM.PlantsGathered += resourceInfo.fruitsPerPlant;
      return true;
   }
}