using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HeadlampScript : MonoBehaviour
{
      public float time;
      public float cooldownTime;
      public Light2D light2D;
      public float lightxoffset;
      public bool IsRight = true;
      public PlayerMovement playerMovement;
      public float flipTime = 0.25f;
      public Rigidbody2D rb;
      
      public Light2D primaryLight;
      public Light2D secondaryLight;
      public bool lastIsRight;


      private void Start()
      {
         //get duration of bounce animation
         Animator animator = GetComponentInParent<Animator>();
         AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
         AnimationClip bounceClip = Array.Find(clips, clip => clip.name == "Player_Bounce"); 
         flipTime = bounceClip.length;      
            
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
      }
}
