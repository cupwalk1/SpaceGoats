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

   public override void RefreshTile(Vector3Int position, ITilemap tilemap)
   {
      base.RefreshTile(position, tilemap);
      if (tilemap.GetTile(position) == null)
      {
         Transform tileTransform = tilemap.GetComponent<Tilemap>().transform.Find(goName);
         if (tileTransform != null)
         {
            Object.DestroyImmediate(tileTransform.gameObject);
         }
      }
   }

   public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
   {
      base.GetTileData(position, tilemap, ref tileData);
      
      if (ResourceManager.Instance != null)
      {
         (ResourceData.ResourceType type, string levelName) = ResourceManager.Instance.GetResourceType(position);
         ResourceData rd = ResourceManager.Instance.GetResource(position, type, levelName);
         tileData.sprite = rd.TimeToRipe <= 0 ? RipeSprite : UnripeSprite;
      }

   }
}