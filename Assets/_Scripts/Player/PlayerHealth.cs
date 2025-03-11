using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
   private PlayerManager _pm;
   
   [SerializeField] float DeathUpForce = 10f;

   // Start is called once before the first execution of Update after the MonoBehaviour is created;
   

   int health
   {
      get { return _pm.Health; }
      set { _pm.Health = value; }
   }
   
   void Start()
   {
      _pm = GetComponent<PlayerManager>();
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
      _pm.Health = _pm.MaxHearts;
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
      Invoke("CallGameEnd", 1f);
   }

   void CallGameEnd()
   {
      GameManager.Instance.GameEnded.Invoke();
   }
}