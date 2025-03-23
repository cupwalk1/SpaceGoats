using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
   private PlayerManager _pm;
   
   [SerializeField] float DeathUpForce = 10f;

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
      _pm.Health = _pm.GoatStats.maxGoatHealth;
   }
   
   

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.CompareTag("Spike"))
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