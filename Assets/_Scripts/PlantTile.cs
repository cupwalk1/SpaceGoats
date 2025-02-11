
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using TileData = UnityEngine.Tilemaps.TileData;

public class PlantTile : Tile
{

   private PlantScript script;
   public GameObject plantTilePrefab;
   [SerializeField] private Light2D light2D;
   [SerializeField] private BoxCollider2D boxCollider;
   public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
   {
      
      string goName = "Plant_" + position;
      Transform existingTransform = tilemap.GetComponent<Tilemap>().transform.Find(goName);
      if (existingTransform == null)
      {
         go = Object.Instantiate(plantTilePrefab, tilemap.GetComponent<Tilemap>().GetCellCenterWorld(position),
            Quaternion.identity);
         go.name = goName;
         go.transform.SetParent(tilemap.GetComponent<Tilemap>().transform);
      }
      else
      {
         go = existingTransform.gameObject;
      }
      
      script = go.GetComponent<PlantScript>();
      script.tilemap = tilemap.GetComponent<Tilemap>();
      script.position = position;
      
      return base.StartUp(position, tilemap, go);
   }

   public override void RefreshTile(Vector3Int position, ITilemap tilemap)
   {
      base.RefreshTile(position, tilemap);
      
      if (tilemap.GetTile(position) == null)
      {
         Transform tileTransform = tilemap.GetComponent<Tilemap>().transform.Find("Plant_" + position);
         if (tileTransform != null)
         {
            Object.DestroyImmediate(tileTransform.gameObject);
         }
      }
   }

   public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
   {
      base.GetTileData(position, tilemap, ref tileData);

      if (PlantManager.Instance != null && PlantManager.Instance.Plants.TryGetValue(position, out var plantData))
      {
         tileData.sprite = plantData.IsRipe ? script.ripeSprite : script.unripeSprite;
      }
   }
}