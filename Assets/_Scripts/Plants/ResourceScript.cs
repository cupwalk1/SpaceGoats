using System;
using System.Collections;
using System.Timers;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class ResourceScript : MonoBehaviour
{
   public TileData tileData;
   public Vector3Int position;
   public SpriteRenderer minimapSprite;
   
   Color readyColor = Color.yellow;
   Color notReadyColor = new Color(140, 140, 100, 1);
   
   Coroutine _regenCoroutine;
   private Coroutine RegenCoroutine
   {
      get => _regenCoroutine;
      set
      {
         if (_regenCoroutine == null)
         {
            _regenCoroutine = value;
         }
      }
      
   }
   public abstract int MaxTimeToRegen { get; set; }

   protected ResourceManager _RM
   {
      get => ResourceManager.Instance;
   }

   private ResourceData _thisResourceData;
   public static ITilemap tilemap;


   private void Start()
   {
      minimapSprite = gameObject.GetComponentInChildren<SpriteRenderer>();
      _thisResourceData = _RM.GetResource(position);
      Debug.Log(_thisResourceData.IsRipe);
      if(!IsRipe)
      {
         minimapSprite.color = notReadyColor;
         RegenCoroutine = StartCoroutine("Regen");
         gameObject.GetComponentInParent<Tilemap>().RefreshTile(position);
      }
      else minimapSprite.color = readyColor;
      var elapsedTime = (DateTime.Now - _RM.lastSave).TotalSeconds;
      TimeToRipe -= elapsedTime;
      
   }

   IEnumerator Regen()
   {
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

   public double TimeToRipe
   {
      get => _thisResourceData.TimeToRipe;
      set
      {
         _thisResourceData.TimeToRipe = value;
         if (value <= 0)
         {
            minimapSprite.color = readyColor;
            gameObject.GetComponentInParent<Tilemap>().RefreshTile(position);
         }
      }
   }


   private void OnDestroy()
   {
      _RM.SaveResources();
      StopCoroutine(RegenCoroutine);
   }

   public abstract bool Harvest();

   public void OnTriggerEnter2D(Collider2D other)
   {
      if (!other.gameObject.transform.parent.CompareTag("Player")) return;
      if (IsRipe)
      {
         if (Harvest())
         {
            minimapSprite.color = notReadyColor;
            TimeToRipe = MaxTimeToRegen;
            RegenCoroutine = StartCoroutine("Regen");
            gameObject.GetComponentInParent<Tilemap>().RefreshTile(position);
         }
      }
      
   }
}