
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
   [SerializeField] float time;

   public Text t1;
   public Text t2;
   public Text t3;
   public Text t4;
   
   private bool queuedJump = false;
   private List<Contacts> lastContacts = new();
   private Contacts bufferContacts = new();
   private Contacts contacts;
   private bool justExitedCollision = false;
   private bool afterWallFall = false;
   
   [SerializeField] private float tolerance;
   [SerializeField] private float graceFramesJump;
   [SerializeField] private float jumpBuffer;
   [SerializeField] private float currentSpeed;
   [SerializeField] private float maxSpeed;
   [SerializeField] private float maxAcceleration;
   [SerializeField] private float maxSlidingSpeed;
   [SerializeField] private float jumpForce;
   [SerializeField] private float wallJumpForce;
   [SerializeField] private float acceleration;
   [SerializeField] private float xJumpForce;
   [SerializeField] private float xTolerance;
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
      lastContacts.Add(new Contacts{x = -1, y = -1});
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

   float GetDistance(Vector2 direction)
   {
      float semiExtentY = (GetComponent<Collider2D>().bounds.extents.y);
      float semiExtentX = (GetComponent<Collider2D>().bounds.extents.x);
      
      
      RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
      RaycastHit2D hit1 = Physics2D.Raycast( new Vector2(direction.y * semiExtentX + transform.position.x, direction.x * semiExtentY + transform.position.y), direction);
      RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(direction.y * -semiExtentX + transform.position.x, direction.x * -semiExtentY + transform.position.y), direction);
      
      if (!hit1) hit1 = new RaycastHit2D() { distance = 10 };
      if (!hit2) hit2 = new RaycastHit2D() { distance = 10 };
      
      return Mathf.Min(hit1.distance, hit2.distance, hit.distance);
   }

   Vector2 GetContacts()
   {
      
      //i want to use rb.cast for this
      Vector2 newContacts = Vector2.zero;

      float downDistance = GetDistance(Vector2.down);
      float leftDistance = GetDistance(Vector2.left);
      float rightDistance = GetDistance(Vector2.right);
      
      t3.text = leftDistance.ToString();
      t4.text = rightDistance.ToString();

      if (Mathf.Abs(downDistance - GetComponent<Collider2D>().bounds.extents.y) < tolerance)
         newContacts.y = -1;
      if (Mathf.Abs(leftDistance - GetComponent<Collider2D>().bounds.extents.x) < tolerance)
         newContacts.x = -1;
      if (Mathf.Abs(rightDistance - GetComponent<Collider2D>().bounds.extents.x) < tolerance)
         newContacts.x = 1;

      t1.text = newContacts.ToString(); 
      
      return newContacts;
   }
   

   void OnCollisionEnter2D(Collision2D collision)
   {
      contacts.contacts = GetContacts();
      
      //if it hits a wall don't bounce
      if (contacts.State == Contacts.PlayerState.Wall) rb.linearVelocityX = 0;
      
      //if the time since jump is less than the jump buffer then jump
      if (time < jumpBuffer)
         HandleJump(new InputAction.CallbackContext());
      
      afterWallFall = false;
      
   }
   
   void OnCollisionExit2D(Collision2D collision)
   {
      justExitedCollision = true;
      contacts.contacts = GetContacts();
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
      queuedJump = true;
      
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


   int CalculateMoveDirection()
   {

      if (lastContacts.Find(w => w.x != 0) == null)
         return 0;
         
      
      if (contacts.State == Contacts.PlayerState.Corner)
         return (int) -contacts.x;  
      
      
      if (contacts.IsGrounded
          && lastContacts.Find(w =>
                w.State == Contacts.PlayerState.Corner ||
                w.State == Contacts.PlayerState.Wall)
             .IsCorner)
         return (int) -lastContacts.Find(w => w.State == Contacts.PlayerState.Corner).x;
      
      
      if(rb.linearVelocityY < 0 && contacts.State == Contacts.PlayerState.Airborne && lastContacts[0].State == Contacts.PlayerState.Wall && justExitedCollision)
      {
         afterWallFall = true;
         return (int)lastContacts[0].x;
      }
      if (afterWallFall)
      {
         return (int)lastContacts[0].x;
      }
      
         
      return 0;
   }
   
   

   private void FixedUpdate()
   {
      //get contacts
      contacts.contacts = GetContacts();

      //if stopped and airborne set grounded
      if (contacts.contacts == Vector2.zero && rb.linearVelocity == Vector2.zero) contacts.y = -1;
      
      //slow falling if falling down a wall
      if (contacts.State == Contacts.PlayerState.Wall && rb.linearVelocityY < 0) 
         rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -maxSlidingSpeed, maxSlidingSpeed);
      
      //only increase time if time is less than 2
      if(time < 2) 
         time += Time.fixedDeltaTime;
      
      //if x velocity is less than xTolerance set it to 0
      if (Mathf.Abs(rb.linearVelocityX) < xTolerance)
         rb.linearVelocityX = 0;
      
      // if its grounded and still, look for the last collision it had.
      if(contacts.State == Contacts.PlayerState.Grounded && rb.linearVelocityX == 0)
      {
            rb.linearVelocityX = -lastContacts.Find(w => w.x != 0).x * maxSpeed;
      }
      
      //when it touches the wall, set x velocity 0
      if (contacts.State == Contacts.PlayerState.Wall)
         rb.linearVelocityX = 0;
      
      
      
      //keep speed at max
      if (CalculateMoveDirection() != 0)
      {
         float moveDirection = CalculateMoveDirection();

         if (contacts.x != 0) moveDirection = -contacts.x;
         
         float targetVelocity = moveDirection * maxSpeed;
         //Find the change of velocity needed to reach target
         float velocityChange = targetVelocity - rb.linearVelocityX;
         //Convert to acceleration, which is change of velocity over time
         acceleration = velocityChange / Time.fixedDeltaTime;
         //Clamp it to your maximum acceleration magnitude
         acceleration = Mathf.Clamp(acceleration, -maxAcceleration, maxAcceleration);
         //Then AddForce
         rb.AddForceX(acceleration, ForceMode2D.Force);
      }

      t2.text = lastContacts[0].contacts.ToString();
      
      
      UpdateLastContacts();          
      UpdateFrameStates(contacts.State);
      justExitedCollision = false;
      
   }
   
   void UpdateLastContacts()
   {
      if (bufferContacts.contacts == contacts.contacts) return;
      lastContacts.Insert(0, new Contacts { contacts = bufferContacts.contacts });
      bufferContacts.contacts = contacts.contacts;
      if (lastContacts.Count > 20) lastContacts.RemoveAt(20);
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