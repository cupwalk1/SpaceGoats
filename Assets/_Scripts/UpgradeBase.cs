using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts
{
   public abstract class UpgradeBase
   {
      public string Name;
      public string Description;
      public int Price;
      
      public abstract void OnEnable();
      public virtual void OnDisable(){}

      public bool IsEnabled { get; private set; } = false;
   }
   public class ExtraLifeUpgrade : UpgradeBase
   {
      public ExtraLifeUpgrade()
      {
         Price = 100;
         Name = "Cuore Extra";
         Description = "Aumenta il numero di cuori massimi di uno";
      }
      public override void OnEnable()
      {
         GameObject.FindWithTag("Player").GetComponent<PlayerManager>().MaxHearts++;
      }
   }
   
   public class IncreaseOxygenCapacityUpgrade : UpgradeBase
   {
      public IncreaseOxygenCapacityUpgrade()
      {
         Price = 100;
         Name = "Aumento Capacità";
         Description = "Aumenta la capacità di ossigeno di 10 secondi";
      }
      public override void OnEnable()
      {
         GameObject.FindWithTag("Player").GetComponent<PlayerManager>().MaxOxygen += 10;
      }
   }
   
   public class IncreasePlantCapacityUpgrade : UpgradeBase
   {
      public IncreasePlantCapacityUpgrade()
      {
         Price = 100;
         Name = "Aumento Capacità";
         Description = "Aumenta la capacità di piante di 1";
      }
      public override void OnEnable()
      {
         GameObject.FindWithTag("Player").GetComponent<PlayerManager>().MaxPlants++;
      }
   }
}