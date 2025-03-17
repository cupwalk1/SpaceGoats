using System;
using UnityEngine;
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
   Animator _animator;
      

   public bool lastIsRight;
      
   [SerializeField] PlayerManager playerManager;
   [SerializeField] Slider oxygenBar;

   private void Start()
   {
      _animator = GetComponent<Animator>();
      GameManager.Instance.GameStart.AddListener(OnGameStart);
      playerManager.OnPlayerDie.AddListener(OnPlayerDie);

   }

   private void OnPlayerDie()
   {
   }

   void OnGameStart()
   {

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
      _animator.SetTrigger("Flip");
   }
   
   public void FlipSprite()
   {
      spriteRenderer.flipX = !IsRight;
   }
}