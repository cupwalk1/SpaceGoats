using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
   private PlayerManager _pm;
   [SerializeField] private Slider oxygenBar;
   
   [SerializeField] float DeathUpForce = 10f;

   // Start is called once before the first execution of Update after the MonoBehaviour is created;
   
   float oxygenLevel
   {
      set
      {
         oxygenBar.value = value; 
      }
      get { return oxygenBar.value; }
   }

   int health
   {
      get { return _pm.Health; }
      set { _pm.Health = value; }
   }
   
   void Start()
   {
      _pm.OnPlayerDie.AddListener(Die);
      GameManager.Instance.GameStart.AddListener(GameStart);

   }

   private void GameStart()
   {
      _pm.EnableMoveJump();
      _pm.ShouldMoveCamera = true;
   }

   public void ResetHealth()
   {
      oxygenLevel = 1;
      _pm.Health = _pm.MaxHearts;
   }
   

   // Update is called once per frame
   void Update()
   {
      if (_pm.IsGameInProgress)
      {
         oxygenLevel -= Time.deltaTime / _pm.MaxOxygen;
         if (oxygenBar.value <= 0)
         {
            _pm.OnPlayerDie.Invoke();
         }
      }
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.name == "Obstacles")
      {
         if (health > 1)
         {
            health -= 1;
            _pm.OnTakeDamage.Invoke();
         }
         else
         {
            health = 0;
            _pm.OnPlayerDie.Invoke();
         }
      }
   }

   private void Die()
   {
      _pm.IsGameInProgress = false;
      //reset velocity
      oxygenLevel = 0;
      var rb = GetComponent<Rigidbody2D>();
      rb.linearVelocityX = 0;
      _pm.DisableMoveJump();
      _pm.ShouldMoveCamera = false;
      Invoke("DieEnd", 1f);
   }

   private void DieEnd()
   {
      var rb = GetComponent<Rigidbody2D>();
      rb.AddForce(Vector2.up * DeathUpForce, ForceMode2D.Impulse);
      GetComponentInChildren<Collider2D>().enabled = false;
   }
}