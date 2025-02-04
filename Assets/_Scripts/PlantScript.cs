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
   
   public int RegenTime
   {
      get
      {
         if (PlantManager.Plants.TryGetValue(position, out var plantData))
            return plantData.TimeToRipe;
         PlantManager.Plants[position] = new PlantData { Position = position};
         return 10;
      }
      private set
      {
         if (PlantManager.Plants.TryGetValue(position, out var plantData))
         {
            plantData.TimeToRipe = value;
            PlantManager.Plants[position] = plantData;
         }
         else
         {
            PlantManager.Plants[position] = new PlantData { Position = position, TimeToRipe = value };
         }
      }
   }

   public Tilemap tilemap;

   

   void Start()
   {
      tilemap.RefreshTile(position);
      PlantManager.LoadPlants();
      PlantManager.StartRegenCounter();
   }

   private void OnDestroy()
   {
      PlantManager.SavePlants();
   }

   private void Harvest()
   {
      if(IsRipe)
         RegenTime = GameObject.Find("Goat").GetComponent<PlayerManager>().plantRegenTime;
      IsRipe = false;
      tilemap.RefreshTile(position);
   }

   public void OnTriggerEnter2D(Collider2D other)
   {
      if (!other.gameObject.CompareTag("Player")) return;
      Harvest();
   }
}