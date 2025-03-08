using UnityEngine;

[System.Serializable]
public class ResourceData
{
   public Vector3Int Position;
   public bool IsRipe
   {
      get => TimeToRipe <= 0;
   }
   public int TimeToRipe;
}