using System;
using Random = UnityEngine.Random;

public class EnergyScript : ResourceScript
{
   public int averageBreakingTime = 120;
   public int randomSemiSpan = 40;

   public override ResourceData.ResourceType Type => ResourceData.ResourceType.Energy;
   
   public override int MaxTimeToRegen
   {
      get
      {
         return Random.Range(averageBreakingTime - randomSemiSpan, averageBreakingTime + randomSemiSpan);
      }
      set => value = averageBreakingTime;
   }

   public override bool Harvest()
   {
      return true;
   }
}