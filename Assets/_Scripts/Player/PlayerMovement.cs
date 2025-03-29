using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public GoatStats goatStats;
    private static readonly int Enum = Animator.StringToHash("enum Index");
    private static readonly int Yvelocity = Animator.StringToHash("Yvelocity");
    [SerializeField] private Transform hitbox;
    [SerializeField] private Animator anim;
    [SerializeField] private float jumpTime, maxJumpTime , jumpBufferTime;
    [SerializeField] private Text t1, t2, t3, t4;
    private PlayerManager _p;

    [SerializeField] private float
        extraHeight,
        tolerance,
        graceFramesJump,
        jumpBuffer,
        currentSpeed,
        maxDefaultAcceleration,
        maxSlidingSpeed;

    [Header("Jump Forces")]
    [SerializeField] private float initialJumpForce;
    private float continualJumpForce => goatStats.jumpForce;
    [SerializeField] private float initalXJumpForce;
    [SerializeField] private float continualXJumpForce;
        
        

    private bool isJumping;


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

    }

    void OnDisable()
    {
        input.Disable();
        jump.Disable();
    }

    bool GetHit(Vector2 direction)
    {
        var collider = GetComponentInChildren<Collider2D>();
        return Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0, direction, extraHeight).collider != null;
    }

    Vector2 GetContacts()
    {
        Vector2 newContacts = Vector2.zero;
        
        if (GetHit(Vector2.down))
            newContacts.y = -1;
        if (GetHit(Vector2.left))
            newContacts.x = -1;
        if (GetHit(Vector2.right))
            newContacts.x = 1;
        if (GetHit(Vector2.up))
            newContacts.y = 1;
        return newContacts;
    }

    void OnCollisionEnter2D()
    {
        contacts.contacts = GetContacts();

        if (contacts.State == Contacts.PlayerState.Wall) rb.linearVelocityX = 0;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name == "Door" && rb.linearVelocityX < 0)
        {
            _p.ShouldJump = false;
            _p.OnDoorThreshold = true;
        } 
        else if(other.gameObject.name == "Mask")
           GameManager.Instance.OnPlayerWin.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.name == "Door")
        {
            _p.ShouldJump = true;
            _p.OnDoorThreshold = false;
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
        
        contacts.contacts = GetContacts();
        anim.SetFloat(Yvelocity, rb.linearVelocity.y);
        anim.SetInteger(Enum, (int)contacts.State);
        if (lastContacts.Count == 0 || lastContacts[0].contacts != contacts.contacts)
        {
            if (IsCausedByJump)  contacts.IsCausedByJump = true;
            else contacts.IsCausedByJump = false;
            
            lastContacts.Insert(0, new Contacts { contacts = contacts.contacts, IsCausedByJump = contacts.IsCausedByJump });
            if (lastContacts.Count > 50) lastContacts.RemoveAt(50);
            IsCausedByJump = false;
            
        }
    
        //reduce sliding speed
        if (contacts.State == Contacts.PlayerState.Wall && rb.linearVelocity.y < 0)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -maxSlidingSpeed, maxSlidingSpeed));
        
    
        if (contacts.State == Contacts.PlayerState.Wall)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        #region Jump
        bool canJumpBuffer = CanJumpBuffer();
        if (jump.WasPressedThisFrame() || canJumpBuffer)
        {
            if (!canJumpBuffer)
            {
                jumpBufferTime = 0;
            }
            if (CanJumpGrounded() && _p.ShouldJump)
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.jump);
                GetComponent<Animator>().SetTrigger("Jump");
                isJumping = true;
                jumpTime = 0;
                IsCausedByJump = true;
                jumpBufferTime = jumpBuffer;
                
                rb.AddForce(new Vector2(0, initialJumpForce ), ForceMode2D.Impulse);
            }
            else if (CanJumpWall() && _p.ShouldJump)
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.jump);
                GetComponent<Animator>().SetTrigger("Jump");
                isJumping = true;
                jumpTime = 0;
                IsCausedByJump = true;
                jumpBufferTime = jumpBuffer;
                
                rb.linearVelocityY = 0;
                rb.AddForce(new Vector2(-contacts.x * initalXJumpForce, initialJumpForce), ForceMode2D.Impulse);
            }
            else
            {
                jumpBufferTime = 0;
            }
        }
        if (jump.IsPressed())
        {
            jumpTime += Time.fixedDeltaTime;
            if (jumpTime < maxJumpTime)
            {
                if(_p.ShouldJump)
                {
                    //get direction to jump
                    //if last contact was a wall, jump in the opposite direction
                    if (lastContacts[0].IsWall)
                        rb.AddForce(new Vector2(-lastContacts[0].x * continualXJumpForce, continualJumpForce));
                    
                    else
                        rb.AddForce(new Vector2(0, continualJumpForce));
                }
            }
            
        }
        if (jump.WasReleasedThisFrame())
        {
            isJumping = false;
        }

        #endregion 
        
        #region ApplyXMovement

        float moveDirection = CalculateMoveDirection();
        if (moveDirection != 0 && _p.ShouldMove)
        {
            float targetVelocity = moveDirection * goatStats.speed;
            float velocityChange = targetVelocity - rb.linearVelocity.x;
            float acceleration = velocityChange / Time.fixedDeltaTime;
            acceleration = Mathf.Clamp(acceleration, -maxAcceleration, maxAcceleration);
            rb.AddForce(new Vector2(acceleration, 0));
            if (Mathf.Abs(Mathf.Abs(rb.linearVelocity.x) - goatStats.speed) < tolerance)
                maxAcceleration = maxDefaultAcceleration;
        }

        #endregion
    
        UpdateFrameStates(contacts.State);
    }

    bool CanJumpBuffer()
    {
        if (jumpBufferTime < jumpBuffer && (CanJumpGrounded() || CanJumpWall()))
        {
            jumpBufferTime = jumpBuffer;
            return true;
        }
        return false;
    }
    
    void UpdateFrameStates(Contacts.PlayerState state)
    {
        frameStates.Insert(0, state);
        if (frameStates.Count > 20) frameStates.RemoveAt(20);
    }

}