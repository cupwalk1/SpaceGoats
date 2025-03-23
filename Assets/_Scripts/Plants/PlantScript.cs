using UnityEngine;
using UnityEngine.Tilemaps;

class PlantScript : ResourceScript
{
   public override int MaxTimeToRegen => resourceInfo.plantRegenTime;

   public override ResourceData.ResourceType Type => ResourceData.ResourceType.Plant;
   public override bool Harvest()
   {
      if(_RM.PlantsGathered+_RM.TotalFood >= resourceInfo.maxFruitsInWarehouse)
      {
         Debug.Log("Plant capacity reached");
         return false;
      }

      if (_RM.PlantsGathered + _RM.TotalFood + resourceInfo.fruitsPerPlant > resourceInfo.maxFruitsInWarehouse)
      {
         _RM.PlantsGathered = resourceInfo.maxFruitsInWarehouse - _RM.TotalFood;
         return true;
      }
      _RM.PlantsGathered += resourceInfo.fruitsPerPlant;
      return true;
   }
}