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

   [MenuItem("Developer/Add Resources/+10 Materials")]
   public static void AddMaterials()
   {
      ResourceManager.Instance.TotalMaterials += 10;
   }
   
   [MenuItem("Developer/Add Resources/+10 Food")]
   public static void AddFood()
   {
      ResourceManager.Instance.TotalFood += 10;
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
   
   [MenuItem("Tools/Remove Missing Scripts from Scene & Prefabs")]
   public static void RemoveAllMissingScripts()
   {
      int sceneCount = RemoveMissingScriptsFromScene();
      int prefabCount = RemoveMissingScriptsFromPrefabs();

      Debug.Log($"Removed missing scripts from {sceneCount} GameObjects in the scene and {prefabCount} prefabs.");
   }

   private static int RemoveMissingScriptsFromScene()
   {
      GameObject[] allObjects = Object.FindObjectsOfType<GameObject>(true);
      int removedCount = 0;

      foreach (GameObject obj in allObjects)
      {
         if (GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj) > 0)
            removedCount++;
      }

      return removedCount;
   }
   
   private static int RemoveMissingScriptsFromPrefabs()
   {
      string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
      int removedCount = 0;

      foreach (string guid in prefabGuids)
      {
         string path = AssetDatabase.GUIDToAssetPath(guid);
         GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

         if (prefab != null)
         {
            if (GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefab) > 0)
            {
               removedCount++;
               EditorUtility.SetDirty(prefab);
            }
         }
      }

      AssetDatabase.SaveAssets();
      return removedCount;
   }
   
   //Set TotalFood to 0
   
   [MenuItem("Developer/Add Resources/Empty Food")]
   public static void EmptyFood()
   {
      ResourceManager.Instance.TotalFood = 0;
   }


}




#endif