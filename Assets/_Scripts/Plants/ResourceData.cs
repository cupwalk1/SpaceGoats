using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ResourceData
{
   
   public ResourceData(ResourceType type, string sceneName, Vector3Int position, double timeToRipe)
   {
      Type = type;
      SceneName = sceneName;
      Position = position;
      TimeToRipe = timeToRipe;
      SetSaveTime(DateTime.Now);
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
   [SerializeField] private string saveTime;
   public Vector3Int Position;
   public bool IsRipe
   {
      get => TimeToRipe <= 0;
   }
   public DateTime GetSaveTime()
   {
      return DateTime.Parse(saveTime); // Convert string back to DateTime
   }
   public void SetSaveTime(DateTime time)
   {
      saveTime = time.ToString("o"); // Convert DateTime to string
   }
   
   public double TimeToRipe;
}