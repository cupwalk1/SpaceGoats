using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private Text t1, t2, t3, t4;
    private PlayerManager PlayerManager;
    [SerializeField] private float tolerance,
        graceFramesJump,
        jumpBuffer,
        currentSpeed,
        maxSpeed,
        maxAcceleration,
        maxSlidingSpeed,
        jumpForce,
        wallJumpForce,
        xJumpForce;

    [SerializeField] private float knockbackForce;
    private List<Contacts> lastContacts = new();
    private Contacts contacts;
    private bool afterWallFall, queueJump, IsCausedByJump;
    private Rigidbody2D rb;
    private InputAction jump;
    private InputSystem_Actions input;
    private List<Contacts.PlayerState> frameStates = new();

    void Start()
    {
        PlayerManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
        contacts = new Contacts();
        lastContacts.Add(new Contacts { x = -1, y = -1 });
        lastContacts.Add(new Contacts { x = 0, y = -1 });
        PlayerManager.OnTakeDamage.AddListener(DamageKnockback);
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
        jump.performed += HandleButton;
    }

    void OnDisable()
    {
        input.Disable();
        jump.Disable();
    }

    float GetDistance(Vector2 direction)
    {
        float semiExtentY = GetComponent<Collider2D>().bounds.extents.y;
        float semiExtentX = GetComponent<Collider2D>().bounds.extents.x;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
        RaycastHit2D hit1 = Physics2D.Raycast(new Vector2(direction.y * semiExtentX + transform.position.x, direction.x * semiExtentY + transform.position.y), direction);
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(direction.y * -semiExtentX + transform.position.x, direction.x * -semiExtentY + transform.position.y), direction);

        if (!hit1) hit1 = new RaycastHit2D { distance = 10 };
        if (!hit2) hit2 = new RaycastHit2D { distance = 10 };

        return Mathf.Min(hit1.distance, hit2.distance, hit.distance);
    }

    Vector2 GetContacts()
    {
        Vector2 newContacts = Vector2.zero;

        float downDistance = GetDistance(Vector2.down);
        float leftDistance = GetDistance(Vector2.left);
        float rightDistance = GetDistance(Vector2.right);
        float upDistance = GetDistance(Vector2.up);

        if (Mathf.Abs(downDistance - GetComponent<Collider2D>().bounds.extents.y) < tolerance)
            newContacts.y = -1;
        if (Mathf.Abs(upDistance - GetComponent<Collider2D>().bounds.extents.y) < tolerance)
            newContacts.y = 1;
        if (Mathf.Abs(leftDistance - GetComponent<Collider2D>().bounds.extents.x) < tolerance)
            newContacts.x = -1;
        if (Mathf.Abs(rightDistance - GetComponent<Collider2D>().bounds.extents.x) < tolerance)
            newContacts.x = 1;

        t1.text = newContacts.ToString();
        return newContacts;
    }

    void OnCollisionEnter2D()
    {
        contacts.contacts = GetContacts();

        if (contacts.State == Contacts.PlayerState.Wall) rb.linearVelocityX = 0;
        
    }
    
    void HandleButton(InputAction.CallbackContext context)
    {
        queueJump = true;

        if (contacts.IsAirborne) time = 0;
        
    }
    void HandleJump()
    {
        contacts.contacts = GetContacts();

        if (contacts.IsCausedByJump) return;

        if (CanJumpGrounded())
        {
            time = jumpBuffer;
            rb.AddForce(Vector2.up * jumpForce);
            if (contacts.IsAirborne) contacts.IsCausedByJump = true;
            else IsCausedByJump = true;
        }
        else if (CanJumpWall())
        {
            time = jumpBuffer;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(new Vector2(-contacts.x * xJumpForce, wallJumpForce));
            if (contacts.IsAirborne) contacts.IsCausedByJump = true;
            else IsCausedByJump = true;
            
        }

    }

    private bool CanJumpWall()
    {
        return contacts.IsWall;
    }

    bool CanJumpGrounded()
    {
        return contacts.IsGrounded || frameStates.Take((int)graceFramesJump).Any(state => state == Contacts.PlayerState.Grounded && rb.linearVelocityY <= 0);
    }

    int CalculateMoveDirection()
    {
        
        //if it can't find a contact with a wall contact, return 0
        if (lastContacts.Find(w => w.x != 0) == null)
            return 0;

        // if the player is in a corner, return the opposite direction
        if (contacts.State == Contacts.PlayerState.Corner)
            return (int)-contacts.x;

        //if the last wall contact is a corner, return the opposite direction of the wall    -    It can't be wall because it wouldn't work for falling off a wall
        if (contacts.IsGrounded && lastContacts.Find(w => w.State is Contacts.PlayerState.Corner or Contacts.PlayerState.Wall).IsCorner)
            return (int)-lastContacts.Find(w => w.IsCorner).x;

        // if it came off a wall and wasn't caused by a jump, return the opposite direction of the wall
        if (contacts.IsAirborne && lastContacts[1].IsWall && !contacts.IsCausedByJump) return (int)lastContacts[1].x;
        
        if (contacts.IsGroundedOrAirborne) return (int) rb.linearVelocity.normalized.x;

        if (contacts.IsWall) return 0;
        //no other conditions are met, return 0
        Debug.LogException(new System.Exception("No conditions were met in CalculateMoveDirection  " + lastContacts[0].State + ",  " + lastContacts[1].State));
        return 0;
    }

    private void FixedUpdate()
    {
        t4.text = PlayerManager.Health.ToString();
        
        if (time < jumpBuffer)
            time += Time.fixedDeltaTime;
        
        contacts.contacts = GetContacts();
        
        if (lastContacts.Count == 0 || lastContacts[0].contacts != contacts.contacts)
        {
            if (IsCausedByJump)  contacts.IsCausedByJump = true;
            else contacts.IsCausedByJump = false;
            
            lastContacts.Insert(0, new Contacts { contacts = contacts.contacts, IsCausedByJump = contacts.IsCausedByJump });
            if (lastContacts.Count > 20) lastContacts.RemoveAt(20);
            IsCausedByJump = false;
            
            if (time < jumpBuffer)
                queueJump = true;
        }
        
        if (queueJump)
        {
            queueJump = false;
            HandleJump();
        }
    
        //reduce sliding speed
        if (contacts.State == Contacts.PlayerState.Wall && rb.linearVelocity.y < 0)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -maxSlidingSpeed, maxSlidingSpeed));
        
        //if the player is grounded and not moving, move in the opposite direction of the wall
        if (contacts.State == Contacts.PlayerState.Grounded && rb.linearVelocity.x == 0)
        {
            rb.linearVelocity = new Vector2(-lastContacts.Find(w => w.x != 0).x * maxSpeed, rb.linearVelocity.y);
        }
    
        if (contacts.State == Contacts.PlayerState.Wall)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        
    
        if (CalculateMoveDirection() != 0 && PlayerManager.ShouldMove)
        {
            float moveDirection = CalculateMoveDirection();
    
            if (contacts.x != 0) moveDirection = -contacts.x;
    
            float targetVelocity = moveDirection * maxSpeed;
            float velocityChange = targetVelocity - rb.linearVelocity.x;
            float acceleration = velocityChange / Time.fixedDeltaTime;
            acceleration = Mathf.Clamp(acceleration, -maxAcceleration, maxAcceleration);
            rb.AddForce(new Vector2(acceleration, 0));
        }
    
        t3.text = contacts.IsCausedByJump.ToString();
    
        UpdateFrameStates(contacts.State);
    }

    
    void UpdateFrameStates(Contacts.PlayerState state)
    {
        frameStates.Insert(0, state);
        if (frameStates.Count > 20) frameStates.RemoveAt(20);
    }

    private void DamageKnockback()
    {
        int direction = (int) -rb.linearVelocity.normalized.x;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(direction * xJumpForce, wallJumpForce));
        
    }
}