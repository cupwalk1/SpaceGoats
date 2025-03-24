#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Scripts;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Developer
{

   [MenuItem("Developer/Add Resources/+100 Materials")]
   public static void AddMaterials()
   {
      ResourceManager.Instance.TotalMaterials += 100;
   }
   
   [MenuItem("Developer/Add Resources/+100 Food")]
   public static void AddFood()
   {
      ResourceManager.Instance.TotalFood += 100;
   }
   
   [MenuItem("Developer/Add Resources/Full Energy")]
   public static void FullEnergy()
   {
      ResourceManager.Instance.Resources.Where(s => s.Type == ResourceData.ResourceType.Energy).ToList().ForEach(s => s.TimeToRipe = ResourceManager.Instance.RandomMaxEnergyTime);
   }
   
   [MenuItem("Developer/Game Over")]
   public static void GameOver()
   {
      GameManager.Instance.OnGameOver.Invoke();
   }

   //victory
   [MenuItem("Developer/Victory")]
   public static void Victory()
   {
      GameManager.Instance.OnGameWin.Invoke();
   }
   
   //Set TotalFood to 0
   
   [MenuItem("Developer/Add Resources/Empty Food")]
   public static void EmptyFood()
   {
      ResourceManager.Instance.TotalFood = 0;
   }


}




#endif