using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace _Scripts
{
   public class UpgradeManager : MonoBehaviour
   {
      public static UpgradeManager Instance;
      GameManager _gm;
      
      private void Awake()
      {
         if (Instance == null)
         {
            Instance = this;
            DontDestroyOnLoad(gameObject);
         }
         else
         {
            Destroy(gameObject);
         }
      }
      
      private void Start()
      {
         _gm = GameManager.Instance;
      }

      public void AddUpgrade(UpgradeBase upgrade)
      {
         GameManager.Instance.gameData.Upgrades.Add(upgrade);
      }
      public void EnableUpgrades()
      {
         var upgrades = GameManager.Instance.gameData.Upgrades;
         foreach (var upgrade in upgrades)
         {
            upgrade.OnEnable();
         }
      }
   }
}