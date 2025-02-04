using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantScript : MonoBehaviour
{
   public TileData tileData;
   public Sprite ripeSprite;
   public Sprite unripeSprite;
   public Vector3Int position;

   public bool IsRipe
   {
      get
      {
         if (PlantManager.Plants.TryGetValue(position, out var plantData))
            return plantData.IsRipe;
         PlantManager.Plants[position] = new PlantData { Position = position, IsRipe = true };
         return true;
      }
      private set
      {
         if (PlantManager.Plants.TryGetValue(position, out var plantData))
         {
            plantData.IsRipe = value;
            PlantManager.Plants[position] = plantData;
         }
         else
         {
            PlantManager.Plants[position] = new PlantData { Position = position, IsRipe = value };
         }
      }
   }

   public Tilemap tilemap;

   

   void Start()
   {
      PlantManager.LoadPlants();

   }

   private void OnDestroy()
   {
      PlantManager.SavePlants();
   }

   private void Harvest()
   {
      if (!IsRipe) return;
      IsRipe = false;
      PlantManager.Plants[position].IsRipe = false;
      tilemap.RefreshTile(position);
   }

   public void OnTriggerEnter2D(Collider2D other)
   {
      if (!other.gameObject.CompareTag("Player")) return;
      Harvest();
   }
}