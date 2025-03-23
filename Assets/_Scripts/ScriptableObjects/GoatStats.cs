   using UnityEngine;
   
   [CreateAssetMenu(fileName = "GoatStats", menuName = "ScriptableObjects/GoatStats")]
   public class  GoatStats : ScriptableObject {
   
      public int maxGoatHealth;
      public int maxTimeOxygen;
      public float speed;

      public void CopyFrom(GoatStats defaultGoatStats)
      {
         maxGoatHealth = defaultGoatStats.maxGoatHealth;
         maxTimeOxygen = defaultGoatStats.maxTimeOxygen;
         speed = defaultGoatStats.speed;
      }
   }