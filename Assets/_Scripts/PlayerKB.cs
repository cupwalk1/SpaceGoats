using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerKB : MonoBehaviour
{
   [SerializeField] private PlayerMovement _playerMovement;
   private bool collided;
   PlayerManager _playerManager;
   private Rigidbody2D _rb;
   [SerializeField]
   private float decceleration, kbAcceleration;
   [SerializeField] private Vector2 backwardKnockbackPower;
   [SerializeField] private Vector2 forwardKnockbackPower;
   private void Start()
   {
      _rb = GetComponent<Rigidbody2D>();
      _playerManager = GetComponent<PlayerManager>();
      _playerManager.OnTakeDamage.AddListener(TakeDamageKB);
   }
   private void TakeDamageKB()
   {
      bool isYzero = Math.Abs(_rb.linearVelocity.y) < 0.01f;
      int Xsign = _playerManager.GetSign(_rb.linearVelocity.x);
      Vector2 knockbackDir = isYzero
         ? new Vector2(-Xsign, 0)
         : new Vector2(Xsign, 1);
      StartCoroutine(Knockback(knockbackDir, !isYzero));
   }
   private IEnumerator Knockback(Vector2 direction, bool isForwardKnockback = false)
   {
      _playerManager.DisableMoveJump();
      _rb.linearVelocity = Vector2.zero;
      yield return new WaitForFixedUpdate();
      _rb.AddForce(direction * (isForwardKnockback ? forwardKnockbackPower : backwardKnockbackPower));
      yield return new WaitForSeconds(0.1f);
      while (_playerManager.GetSign(_rb.linearVelocity.x) == _playerManager.GetSign(direction.x) && !isForwardKnockback)
      {
         yield return new WaitForFixedUpdate();
         _rb.AddForce(-direction * decceleration);
      }
      _playerMovement.maxAcceleration = kbAcceleration;
      _playerManager.EnableMoveJump();
   }

}