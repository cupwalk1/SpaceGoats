using UnityEngine;
using UnityEngine.Rendering.Universal;

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