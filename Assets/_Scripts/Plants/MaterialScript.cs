using UnityEngine;

public class MaterialScript : ResourceScript
{
   public override ResourceData.ResourceType Type => ResourceData.ResourceType.Material;
   public override int MaxTimeToRegen { get; set; } = 15;
   public override bool Harvest()
   {
      ResourceManager.Instance.MaterialsGathered++;
      return true;
   }
}
