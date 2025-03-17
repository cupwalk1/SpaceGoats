using System;
using System.Collections;
using System.Linq;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public abstract class ResourceScript : MonoBehaviour
{
   public abstract ResourceData.ResourceType Type { get; }
   public TileData tileData;
   public Vector3Int position;
   public Image minimapSprite;
   protected Animator _animator;
   
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
      _thisResourceData = _RM.GetResource(position, Type);
      _animator = GetComponent<Animator>();
      minimapSprite = gameObject.GetComponentInChildren<Image>();
      Debug.Log(_thisResourceData.IsRipe);
      if(!IsRipe)
      {
         minimapSprite.color = notReadyColor;
         RegenCoroutine = StartCoroutine("Regen");
      }
      else minimapSprite.color = readyColor;
      var elapsedTime = (DateTime.Now - _thisResourceData.GetSaveTime()).TotalSeconds;
      TimeToRipe -= elapsedTime;
      _animator.SetInteger("TimeToRipe", (int)TimeToRipe);
      
   }

   IEnumerator Regen()
   {
      while (TimeToRipe > 0)
      {
         TimeToRipe--;
         _animator.SetInteger("TimeToRipe", (int)TimeToRipe);
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
         }
      }
   }


   private void OnDestroy()
   {
      _RM.SaveResources();
      if (RegenCoroutine == null) return;
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