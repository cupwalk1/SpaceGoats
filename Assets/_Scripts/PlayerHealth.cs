using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PlayerHealth : MonoBehaviour
{
   private PlayerManager PlayerManager;

   [SerializeField] float DeathUpForce = 10f;

   // Start is called once before the first execution of Update after the MonoBehaviour is created;
   [SerializeField] int spikeDamage = 1;
   [SerializeField] private List<GameObject> obstacles = new();
   [SerializeField] float oxygenLevel;

   int health
   {
      get { return PlayerManager.Health; }
      set { PlayerManager.Health = value; }
   }
   
   void Start()
   {
      PlayerManager = GetComponent<PlayerManager>();
      health = PlayerManager.MaxHearts;
      PlayerManager.OnPlayerDie.AddListener(Die);
   }

   public void ResetHealth()
   {
      PlayerManager.Health = PlayerManager.MaxHearts;
   }

   // Update is called once per frame
   void Update()
   {
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.name == "Obstacles")
      {
         if (health > 1)
         {
            health -= spikeDamage;
            PlayerManager.OnTakeDamage.Invoke();
         }
         else
         {
            health = 0;
            PlayerManager.OnPlayerDie.Invoke();
         }
      }
   }

   private void Die()
   {
      //reset velocity
      var rb = GetComponent<Rigidbody2D>();
      PlayerManager.DisableMoveJump();
      rb.linearVelocity = Vector2.zero;
      PlayerManager.ShouldMoveCamera = false;
      Invoke("DieEnd", 1f);
   }

   private void DieEnd()
   {
      var rb = GetComponent<Rigidbody2D>();
      rb.AddForce(Vector2.up * DeathUpForce, ForceMode2D.Impulse);
      GetComponent<Collider2D>().enabled = false;
   }
}