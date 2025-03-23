using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts
{
   public abstract class UpgradeBase
   {
      public GameObject UpgradePanel;
      public int Level;
      public int MaxLevel;
      public string Name;
      public string Description;
      public int Price;
      public GameObject Panel;
      public ResouceTypes ResourceType1;
      public ResouceTypes ResourceType2;
      public UpgradeScript Script;
      
      public UpgradeBase()
      {
         Level = 0;
         Panel = GameObject.Instantiate(UpgradePanel, GameObject.FindWithTag("UpgradePanel").transform);
         Script = Panel.GetComponent<UpgradeScript>();
         Script.BuyButton.onClick.AddListener(() => {
           if(Level < MaxLevel)
           {
              Level++;
               LevelUp();
           }
         });
         
      }
      
      public enum ResouceTypes
      {
         Energy,
         Food,
         Material
      }

      
      
      public abstract void LevelUp();
      
      public abstract void OnEnable();
      
      public bool IsEnabled { get; private set; } = false;
   }
}