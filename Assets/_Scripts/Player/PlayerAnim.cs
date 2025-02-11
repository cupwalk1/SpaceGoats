using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerAnim : MonoBehaviour
{
   public float time;
   public float cooldownTime;
   public float lightxoffset;
   public bool IsRight = true;
   public PlayerMovement playerMovement;
   public SpriteRenderer spriteRenderer;
   public float flipTime = 0.25f;
   public Rigidbody2D rb;
      

   public bool lastIsRight;
      
   [SerializeField] PlayerManager playerManager;
   [SerializeField] Slider oxygenBar;

   private void Start()
   {
      GameManager.Instance.GameStart.AddListener(OnGameStart);
      playerManager.OnPlayerDie.AddListener(OnPlayerDie);
      oxygenBar.handleRect.GetComponent<ParticleSystem>().Stop();
   }

   private void OnPlayerDie()
   {
      oxygenBar.handleRect.GetComponent<ParticleSystem>().Stop();
   }

   void OnGameStart()
   {
      IsRight = true;
      lastIsRight = true;
      oxygenBar.handleRect.GetComponent<ParticleSystem>().Play();
   }

   private void FixedUpdate()
   {
      time += Time.deltaTime;
         
      if (IsRight != lastIsRight)
      {
         if (time > cooldownTime)
         {
            lastIsRight = IsRight;
            time = 0;
            Flip();
            return;
         }
            
      }
      if(Mathf.Abs(rb.linearVelocityX) < 0.1f) 
      {
         IsRight = playerMovement.CurrentContacts.contacts.x < 0;
      }
      else IsRight = rb.linearVelocityX > 0;
   }

   void Flip()
   {
      if(Mathf.Abs(rb.linearVelocityX) < 0.1f) 
      {
         IsRight = playerMovement.CurrentContacts.contacts.x < 0;
      }
      else IsRight = rb.linearVelocityX > 0;
      LeanTween.scaleX(gameObject, IsRight ? 1 : -1, flipTime).setEaseInOutSine();
   }
}