using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnergyScript : ResourceScript
{
   public override ResourceData.ResourceType Type => ResourceData.ResourceType.Energy;

   public override int MaxTimeToRegen => Random.Range(resourceInfo.averageGeneratorBreakingTime - resourceInfo.semiRangeGeneratorBreakingTime,
            resourceInfo.averageGeneratorBreakingTime + resourceInfo.semiRangeGeneratorBreakingTime);
   
   public override bool Harvest()
   {
      Debug.Log("Harvesting energy");
      return true;
   }
}