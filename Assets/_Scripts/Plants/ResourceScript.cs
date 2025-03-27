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
   public ResourceInfo resourceInfo;
   public abstract ResourceData.ResourceType Type { get; }
   public TileData tileData;
   public Vector3Int position;
   public Image minimapSprite;
   protected Animator _animator;

   public Color readyColor = Color.blue;
   public Color notReadyColor = Color.gray;
   
   public abstract int MaxTimeToRegen { get; }

   protected ResourceManager _RM
   {
      get => ResourceManager.Instance;
   }

   private ResourceData _thisResourceData;
   public static ITilemap tilemap;


   private void Start()
   {

      _thisResourceData = _RM.GetResource(transform.position, Type);
      _thisResourceData.ResourceGameObject = gameObject;
      _animator = GetComponent<Animator>();
      minimapSprite = gameObject.GetComponentInChildren<Image>();
      Debug.Log(_thisResourceData.IsRipe);
      if(!IsRipe)
      {
         minimapSprite.color = notReadyColor;
      }
      else minimapSprite.color = readyColor;
      _animator.SetFloat("TimeToRipe", TimeToRipe);
      
   }

   IEnumerator Animate()
   {
         _animator.SetInteger("TimeToRipe", (int) TimeToRipe);
         yield return new WaitForSeconds(1);
   }


   public bool IsRipe
   {
      get => TimeToRipe == 0;
   }

   public float TimeToRipe
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
   }

   public abstract bool Harvest();

   public void OnTriggerEnter2D(Collider2D other)
   {
      if (!other.gameObject.transform.parent.CompareTag("Player")) return;
      if (IsRipe)
      {
         if (Harvest())
         {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.AquiredResource);
            minimapSprite.color = notReadyColor;
            TimeToRipe = MaxTimeToRegen;
            _animator.SetInteger("TimeToRipe", (int)TimeToRipe);
         }
      }
      
   }
}