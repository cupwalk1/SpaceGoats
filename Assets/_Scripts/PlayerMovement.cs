using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private bool IsTouchingSomething;
    [SerializeField] private float time;
    [SerializeField] private Text t1, t2, t3, t4;
    private PlayerManager _p;
    [SerializeField] private float 
        tolerance,
        graceFramesJump,
        jumpBuffer,
        currentSpeed,
        maxSpeed,
        maxDefaultAcceleration,
        maxSlidingSpeed,
        jumpForce,
        wallJumpForce,
        xJumpForce;

    public List<Contacts> LastContacts
    {
        get => lastContacts;
    }
    public Contacts CurrentContacts
    {
        get => contacts;
    }
    
    [SerializeField] private float knockbackForce;
    private List<Contacts> lastContacts = new();
    [HideInInspector] public float maxAcceleration;
    private Contacts contacts;
    private bool afterWallFall, queueJump, IsCausedByJump;
    private Rigidbody2D rb;
    private InputAction jump;
    private InputSystem_Actions input;
    private List<Contacts.PlayerState> frameStates = new();

    void Start()
    {
        _p = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
        
        maxAcceleration = maxDefaultAcceleration;
        
        contacts = new Contacts();
        lastContacts.Add(new Contacts { x = -1, y = -1 });
        lastContacts.Add(new Contacts { x = 0, y = -1 });

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
        RaycastHit2D hit1 = Physics2D.Raycast(new Vector2(direction.y * semiExtentX + transform.position.x, direction.x * semiExtentY*0.9f+ transform.position.y), direction);
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(direction.y * -semiExtentX + transform.position.x, direction.x * -semiExtentY*0.9f + transform.position.y), direction);

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

        if (Mathf.Abs(downDistance - GetComponent<Collider2D>().bounds.extents.y) < tolerance && GetComponent<Collider2D>().IsTouchingLayers())
            newContacts.y = -1;
        if (Mathf.Abs(upDistance - GetComponent<Collider2D>().bounds.extents.y) < tolerance && GetComponent<Collider2D>().IsTouchingLayers())
            newContacts.y = 1;
        if (Mathf.Abs(leftDistance - GetComponent<Collider2D>().bounds.extents.x) < tolerance && GetComponent<Collider2D>().IsTouchingLayers())
            newContacts.x = -1;
        if (Mathf.Abs(rightDistance - GetComponent<Collider2D>().bounds.extents.x) < tolerance && GetComponent<Collider2D>().IsTouchingLayers())
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
        
        if (_p.ShouldMove == false) return;
        
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
        
        
        for (int i = 1; i < lastContacts.Count; i++)
        {
            if (!contacts.IsGrounded) break;
            if (lastContacts[i].State != Contacts.PlayerState.Wall) continue;
            if(lastContacts[i - 1].IsCausedByJump && 
               lastContacts[i - 1].State == Contacts.PlayerState.Airborne)
                return (int)-lastContacts[i].x;
            break;
        }
        // if it came off a wall and wasn't caused by a jump, return the opposite direction of the wall
        if (contacts.IsAirborne && lastContacts[1].IsWall && !contacts.IsCausedByJump) return (int)lastContacts[1].x;
        
        if (contacts.IsGroundedOrAirborne) return _p.GetSign(rb.linearVelocity.x);

        if (contacts.IsWall) return 0;
        //no other conditions are met, return 0
        Debug.LogException(new System.Exception("No conditions were met in CalculateMoveDirection  " + lastContacts[0].State + ",  " + lastContacts[1].State));
        return 0;
    }

    private void FixedUpdate()
    {
        t4.text = _p.Health.ToString();
        
        if (time < jumpBuffer)
            time += Time.fixedDeltaTime;
        
        contacts.contacts = GetContacts();
        
        if (lastContacts.Count == 0 || lastContacts[0].contacts != contacts.contacts)
        {
            if (IsCausedByJump)  contacts.IsCausedByJump = true;
            else contacts.IsCausedByJump = false;
            
            lastContacts.Insert(0, new Contacts { contacts = contacts.contacts, IsCausedByJump = contacts.IsCausedByJump });
            if (lastContacts.Count > 50) lastContacts.RemoveAt(50);
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
        
    
        if (contacts.State == Contacts.PlayerState.Wall)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        
        float moveDirection = CalculateMoveDirection();
        if (moveDirection != 0 && _p.ShouldMove)
        {
            float targetVelocity = moveDirection * maxSpeed;
            float velocityChange = targetVelocity - rb.linearVelocity.x;
            float acceleration = velocityChange / Time.fixedDeltaTime;
            acceleration = Mathf.Clamp(acceleration, -maxAcceleration, maxAcceleration);
            rb.AddForce(new Vector2(acceleration, 0));
            if (Mathf.Abs(Mathf.Abs(rb.linearVelocity.x) - maxSpeed) < tolerance)
                maxAcceleration = maxDefaultAcceleration;
        }
    
    
        UpdateFrameStates(contacts.State);
    }

    
    void UpdateFrameStates(Contacts.PlayerState state)
    {
        frameStates.Insert(0, state);
        if (frameStates.Count > 20) frameStates.RemoveAt(20);
    }
    
}