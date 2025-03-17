using UnityEngine;
using UnityEngine.Tilemaps;

class PlantScript : ResourceScript
{
   public override int MaxTimeToRegen { get; set; } = 10;

   public override ResourceData.ResourceType Type => ResourceData.ResourceType.Plant;
   public override bool Harvest()
   {
      if(ResourceManager.Instance.PlantsGathered+ResourceManager.Instance.TotalFood >= ResourceManager.Instance.PlantMaxCapacity)
      {
         Debug.Log("Plant capacity reached");
         return false;
      }
      ResourceManager.Instance.PlantsGathered++;
      return true;
   }
}