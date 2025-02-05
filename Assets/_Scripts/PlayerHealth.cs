using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PlayerHealth : MonoBehaviour
{
   private PlayerManager _pm;

   [SerializeField] float DeathUpForce = 10f;

   // Start is called once before the first execution of Update after the MonoBehaviour is created;
   [SerializeField] int spikeDamage = 1;
   [SerializeField] private List<GameObject> obstacles = new();
   [SerializeField] float oxygenLevel;

   int health
   {
      get { return _pm.Health; }
      set { _pm.Health = value; }
   }
   
   void Start()
   {
      _pm = GetComponent<PlayerManager>();
      health = _pm.MaxHearts;
      _pm.OnPlayerDie.AddListener(Die);
   }

   public void ResetHealth()
   {
      _pm.Health = _pm.MaxHearts;
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
      //reset velocity
      var rb = GetComponent<Rigidbody2D>();
      _pm.DisableMoveJump();
      rb.linearVelocity = Vector2.zero;
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