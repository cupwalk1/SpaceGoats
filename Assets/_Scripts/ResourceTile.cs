using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;


public class ResourceTile : Tile
{
   [Header("Resource Settings")] public String Name;
   public Sprite RipeSprite;
   public Sprite UnripeSprite;
   public GameObject Prefab;

   [Space] private string goName;

   public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
   {
      goName = $"{Name} {position}";
      if (go == null) return base.StartUp(position, tilemap, go);
      go.name = goName;
      go.GetComponent<ResourceScript>().Initialize(position, tilemap);

      return base.StartUp(position, tilemap, go);
   }
}