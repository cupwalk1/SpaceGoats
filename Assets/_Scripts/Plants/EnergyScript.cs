using System;
using Random = UnityEngine.Random;

public class EnergyScript : ResourceScript
{
   public int averageBreakingTime = 10;
   public int randomSemiSpan = 5;

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