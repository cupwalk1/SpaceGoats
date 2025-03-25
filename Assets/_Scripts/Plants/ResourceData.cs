using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ResourceData
{
   
   public ResourceData(ResourceType type, string sceneName, Vector3 position, double timeToRipe)
   {
      Type = type;
      SceneName = sceneName;
      Position = position;
      TimeToRipe = timeToRipe;
   }
   
   public enum ResourceType
   {
      Energy,
      Material,
      Plant, 
      None
   }
   public ResourceType Type;
   public string SceneName;
   public Vector3 Position;
   public bool IsRipe
   {
      get => TimeToRipe == 0;
   }
   public double TimeToRipe;
}