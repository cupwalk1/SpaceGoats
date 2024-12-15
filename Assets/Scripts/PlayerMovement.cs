using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
   private PlayerInputHandler playerInputHandler;

   void Start()
   {
      playerInputHandler = gameObject.AddComponent<PlayerInputHandler>();
      playerInputHandler.OnJump(new InputAction.CallbackContext());
   }
}