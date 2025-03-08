using UnityEngine;

public class MaterialScript : ResourceScript
{
   public override int MaxTimeToRegen { get; set; } = 10;
   public override bool Harvest()
   {
      return true;
   }
}
