using UnityEngine;
using System.Collections;

public class PlayerImmunity : MonoBehaviour
{
      [SerializeField] private float ImmunityTime;
      private PlayerManager _playerManager;
      private void Start()
      {
         _playerManager = GetComponent<PlayerManager>();
         _playerManager.OnTakeDamage.AddListener(TakeDamageImmunity);
      }
   
      private void TakeDamageImmunity()
      {
         StartCoroutine(Immunity());
      }
   
      private IEnumerator Immunity()
      {
         _playerManager.IsImmune = true;
         yield return new WaitForSeconds(ImmunityTime);
         _playerManager.IsImmune = false;
      }
}
