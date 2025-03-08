using UnityEngine;
using UnityEngine.Tilemaps;

class PlantScript : ResourceScript
{
   public override int MaxTimeToRegen { get; set; } = 10;

   public override bool Harvest()
   {
      if (_RM.PlantsGatheredDuringRun < GameObject.Find("Goat").GetComponent<PlayerManager>().MaxPlants)
      {
         GameManager.Instance.gameData.PlantsGatheredDuringRun++;
         _RM.OnPlantGathered();
         return true;
      }

      return false;
   }
}