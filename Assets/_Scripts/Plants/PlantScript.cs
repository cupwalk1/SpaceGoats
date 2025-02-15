using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantScript : MonoBehaviour
{
   public TileData tileData;
   public Sprite ripeSprite;
   public Sprite unripeSprite;
   public Vector3Int position;
   private PlantManager _pm;
   public bool IsRipe
   {
      get
      {
         if (_pm.Plants.TryGetValue(position, out var plantData))
            return plantData.IsRipe;
         _pm.Plants[position] = new PlantData { Position = position, IsRipe = true };
         return true;
      }
      private set
      {
         if (_pm.Plants.TryGetValue(position, out var plantData))
         {
            plantData.IsRipe = value;
            _pm.Plants[position] = plantData;
         }
         else
         {
            _pm.Plants[position] = new PlantData { Position = position, IsRipe = value };
         }
      }
   }
   
   public int RegenTime
   {
      get
      {
         if (_pm.Plants.TryGetValue(position, out var plantData))
            return plantData.TimeToRipe;
         _pm.Plants[position] = new PlantData { Position = position};
         return 10;
      }
      private set
      {
         if (_pm.Plants.TryGetValue(position, out var plantData))
         {
            plantData.TimeToRipe = value;
            _pm.Plants[position] = plantData;
         }
         else
         {
            _pm.Plants[position] = new PlantData { Position = position, TimeToRipe = value };
         }
      }
   }

   public Tilemap tilemap;

   

   void Start()
   {
      tilemap.RefreshTile(position);
      _pm = PlantManager.Instance;
      _pm.LoadPlants();
      _pm.StartRegenCounter();
   }

   private void OnDestroy()
   {
      _pm.SavePlants();
   }

   private void Harvest()
   {
      if(IsRipe && _pm.PlantsGatheredDuringRun < GameObject.Find("Goat").GetComponent<PlayerManager>().MaxPlants)
      {
         
         RegenTime = GameObject.Find("Goat").GetComponent<PlayerManager>().plantRegenTime;
         GameManager.Instance.gameData.PlantsGatheredDuringRun++;      
         IsRipe = false;
         _pm.OnPlantGathered();
      }
      tilemap.RefreshTile(position);
   }

   public void OnTriggerEnter2D(Collider2D other)
   {
      if (!other.gameObject.CompareTag("Player")) return;
      Harvest();
   }
}