using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class PlayerMovement : MonoBehaviour
{
   [SerializeField] float time;
   
   private List<Contacts> lastContacts = new();
   private Contacts bufferContacts = new();
   
   private float vX;
   private Contacts contacts;
   [SerializeField] private float graceFramesJump;
   [SerializeField] private float jumpBuffer;
   [SerializeField] private float currentSpeed;
   [SerializeField] private float maxSpeed;
   [SerializeField] private float maxSlidingSpeed;
   [SerializeField] private float jumpForce;
   [SerializeField] private float wallJumpForce;
   [SerializeField] private float acceleration;
   [SerializeField] private float xJumpForce;
   [SerializeField] private float xTolerance;
   Dictionary<Collider2D, Vector2> currentCollisionsDict = new();
   Rigidbody2D rb;
   InputAction jump;
   List<Contacts.PlayerState> frameStates = new();
   private InputSystem_Actions input;
   private float colliderWidth;
   private float colliderHeight;
   
   void Start()
   {
      colliderWidth = GetComponent<Collider2D>().bounds.size.x;
      contacts = new Contacts();
      colliderHeight = GetComponent<Collider2D>().bounds.size.y;
      rb = GetComponent<Rigidbody2D>();
      lastContacts.Add(new Contacts{x = 0, y = -1});
   }
   
   void Awake()
   {
      input = new InputSystem_Actions();
      jump = input.Player.Jump;
   }
   
   void OnEnable()
   {
      input.Enable();
      jump.Enable();
      jump.performed += HandleJump;
   }
   void OnDisable()
   {
      input.Disable();
      jump.Disable();
   }

   //Vector2(x, y) if touching ground (0, -1) if touching right wall (1, 0)  if touching left wall (-1, 0)  if touching corner (1 or -1, -1) if airborne (0, 0)


   Vector2 GetContacts(Collision2D collision)
   {
      Vector2 newContacts = Vector2.zero;
      Vector2 contactPoint = collision.GetContact(0).point;
      Vector2 localContactPoint = contactPoint - (Vector2)transform.position;
      if (localContactPoint.y < -(colliderHeight / 2) * .9f)  newContacts.y = -1;
      if (localContactPoint.x > (colliderWidth / 2) * .9f)  newContacts.x = 1;
      if (localContactPoint.x < -(colliderWidth / 2) * .9f) newContacts.x = -1;
      return newContacts;
   }
   
   void AddContacts(Vector2 newContacts)
   {
      if(newContacts.y !=0) contacts.y = -1;
      if(newContacts.x !=0) contacts.x = newContacts.x;
      UpdateLastContacts();
   }
   
   void SubtractContacts(Vector2 newContacts)
   {
      if(newContacts.y !=0) contacts.y = 0;
      if(newContacts.x !=0) contacts.x = 0;
      UpdateLastContacts();
   }

   void OnCollisionEnter2D(Collision2D collision)
   {
      Vector2 newContacts = GetContacts(collision);
      currentCollisionsDict.Add(collision.collider, newContacts);
      AddContacts(newContacts);
      
      if (time < jumpBuffer)
      {
         HandleJump(new InputAction.CallbackContext());
      }
      
   }
   
   void OnCollisionExit2D(Collision2D collision)
   {
      SubtractContacts(currentCollisionsDict[collision.collider]);
      currentCollisionsDict.Remove(collision.collider);
      
      if(rb.linearVelocityY < 0 && lastContacts[0].State == Contacts.PlayerState.Wall && contacts.State == Contacts.PlayerState.Airborne)
      {
         AddForceX(acceleration*lastContacts[0].x);
      }
      
   }

   void UpdateVelocity()
   {
      if (contacts.State == Contacts.PlayerState.Wall && rb.linearVelocityY < 0) 
      {
         rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -maxSlidingSpeed, maxSlidingSpeed);
      }
      
         
      //if player is airborne and last contact was grounded or player is grounded
      if ((contacts.State == Contacts.PlayerState.Airborne && lastContacts[0].State == Contacts.PlayerState.Grounded) ||
          contacts.State == Contacts.PlayerState.Grounded)
      {
         if (Mathf.Abs(rb.linearVelocityX) < maxSpeed)
         {
            switch (rb.linearVelocityX)
            {
               case < 0:
                  AddForceX(-acceleration);
                  break;
               case > 0:
                  AddForceX(acceleration);
                  break;
               default:
                  if(lastContacts.FirstOrDefault(w => w.State == Contacts.PlayerState.Wall) == null)
                  {
                     break;
                  }
                  AddForceX(acceleration * lastContacts.FirstOrDefault(w => w.State == Contacts.PlayerState.Wall)!.x);
                  break;
            }
         }
      }
   }

   void HandleJump(InputAction.CallbackContext context)
   {
      time = 2;
      if (CanJumpGrounded())
      {
         rb.AddForce(new Vector2(0, jumpForce));
         return;
      }

      if (contacts.State == Contacts.PlayerState.Airborne)
      {
         time = 0;
      }
      
      if(contacts.State == Contacts.PlayerState.Wall)
      {
         rb.linearVelocityY = 0;
         rb.AddForce(new Vector2(-contacts.x * xJumpForce, wallJumpForce));
      }
      
   }    
   
   bool CanJumpGrounded()
   {
      if(contacts.State == Contacts.PlayerState.Grounded) return true;
      for (int i = 0; i < graceFramesJump; i++)
      {
         if(i >= frameStates.Count) return false;
         if (frameStates[i] == Contacts.PlayerState.Grounded && rb.linearVelocityY <= 0) return true;
      }
      return false;
   }
   
   
   

   private void FixedUpdate()
   {
      if(time < 2) time += Time.fixedDeltaTime;
      
      
      if (Mathf.Abs(rb.linearVelocityX) < xTolerance) rb.linearVelocityX = 0;
      if (contacts.x != 0 && (int) contacts.y == -1) AddForceX(-contacts.x * acceleration);
      
      UpdateLastContacts();
      UpdateVelocity();
      UpdateFrameStates(contacts.State);
      
      rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSpeed, maxSpeed);
   }
   
   void UpdateLastContacts()
   {
      if (bufferContacts.contacts == contacts.contacts) return;
      lastContacts.Insert(0 ,new Contacts { contacts = bufferContacts.contacts });
      bufferContacts.contacts = contacts.contacts;
      if (lastContacts.Count > 5) lastContacts.RemoveAt(5);
   }
   
   void AddForceX(float x)
   {
      if(Mathf.Abs(rb.linearVelocityX) < maxSpeed) rb.AddForce(new Vector2(x, 0));
      rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSpeed, maxSpeed);
   }
   
   void UpdateFrameStates(Contacts.PlayerState state)
   {
      frameStates.Insert(0,state);   
      if (frameStates.Count > 20) frameStates.RemoveAt(20);
   }
   
}

// (0, -1) <-- contacts
// (0, -1) <-- lastContacts
// (0, -1) <-- bufferContacts

// if buffercontacts != contacts

// lastContacts = contacts

// bufferContacts = contacts