using System;
using UnityEngine;

public class MaterialScript : ResourceScript
{
   public override ResourceData.ResourceType Type => ResourceData.ResourceType.Material;
   public override int MaxTimeToRegen => resourceInfo.oreRegenTime;
   public override bool Harvest()
   {
      ResourceManager.Instance.MaterialsGathered += resourceInfo.materialsPerOre;
      return true;
   }
   
}
