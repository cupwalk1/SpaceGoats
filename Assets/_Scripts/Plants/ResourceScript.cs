using System;
using System.Collections;
using System.Timers;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class ResourceScript : MonoBehaviour
{
   public TileData tileData;
   public Vector3Int position;

   public abstract int MaxTimeToRegen { get; set; }

   protected ResourceManager _RM
   {
      get => ResourceManager.Instance;
   }

   private ResourceData _thisResourceData;
   public static ITilemap tilemap;


   private void Start()
   {
      _thisResourceData = _RM.GetResource(position);
      var elapsedTime = (DateTime.Now - _RM.lastSave).Seconds;
      TimeToRipe -= elapsedTime;
   }

   IEnumerator Regen()
   {
      TimeToRipe = MaxTimeToRegen;
      while (TimeToRipe > 0)
      {
         TimeToRipe--;
         yield return new WaitForSeconds(1);
      }
   }

   public void Initialize(Vector3Int position, ITilemap tilemap)
   {
      this.position = position;
      if (ResourceScript.tilemap is null) ResourceScript.tilemap = tilemap;
   }


   public bool IsRipe
   {
      get => TimeToRipe <= 0;
   }

   public int TimeToRipe
   {
      get => _thisResourceData.TimeToRipe;
      set
      {
         _thisResourceData.TimeToRipe = value;
         if (value <= 0)
         {
            gameObject.GetComponentInParent<Tilemap>().RefreshTile(position);
         }
      }
   }


   private void OnDestroy()
   {
      _RM.SaveResources();
   }

   public abstract bool Harvest();

   public void OnTriggerEnter2D(Collider2D other)
   {
      if (!other.gameObject.CompareTag("Player")) return;
      if (IsRipe)
      {
         if (Harvest())
         {
            StartCoroutine("Regen");
            gameObject.GetComponentInParent<Tilemap>().RefreshTile(position);
         }
      }

      else Debug.Log(TimeToRipe.ToString());
   }
}