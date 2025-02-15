using System;
using _Scripts;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
   public bool IsGameInProgress;
   public int MaxPlants = 5;
   public int MaxHearts = 4;
   public int MaxOxygen = 150;
   public int plantRegenTime = 10;
   public bool IsImmune;
   public UnityEvent OnTakeDamage = new UnityEvent();
   public UnityEvent OnPlayerDie = new UnityEvent();

   [SerializeField] private bool _shouldMove;

   public bool ShouldMove
   {
      get => _shouldMove;
      set => _shouldMove = value;
   }

   [SerializeField] private bool _shouldJump;

   public bool ShouldJump
   {
      get => _shouldJump;
      set => _shouldJump = value;
   }
   
   [SerializeField] private bool _shouldMoveCamera = true;
   public bool ShouldMoveCamera
   {
      get => _shouldMoveCamera;
      set => _shouldMoveCamera = value;
   }

   public void DisableMoveJump()
   {
      _shouldMove = false;
      _shouldJump = false;
   }

   public void EnableMoveJump()
   {
      _shouldMove = true;
      _shouldJump = true;
   }

   [SerializeField] private int _health;

   public int Health
   {
      get => _health;
      set { _health = Mathf.Clamp(value, 0, MaxHearts); }
   }

   public int GetSign(float value, float tolerance = 0.001f)
   {
      if (value < -tolerance)
         return -1;

      else if (value > tolerance)
         return 1;
      else
         return 0;
   }

   private void Start()
   {  
      DisableMoveJump();
      GameManager.Instance.GameStart.AddListener(delegate { IsGameInProgress = true; });
      OnPlayerDie.AddListener(delegate { IsGameInProgress = false; });
      GameManager.Instance.GameLoaded.AddListener(OnGameLoad);
   }
   
   private void OnGameLoad()
   {
      IsGameInProgress = false;

      UpgradeManager.Instance.EnableUpgrades();
      transform.position = GameObject.FindWithTag("Respawn").transform.position;
      GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
      GetComponent<PlayerHealth>().ResetHealth();
      ShouldMoveCamera = true;
      EnableMoveJump();
      GetComponentInChildren<Collider2D>().enabled = true;
   }
   
   
}