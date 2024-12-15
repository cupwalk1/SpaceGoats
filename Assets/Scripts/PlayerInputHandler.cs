using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
   private InputSystem_Actions inputActions;

   private void Awake()
   {
      inputActions = new InputSystem_Actions();
      inputActions.Player.SetCallbacks(this);
   }

   private void OnEnable()
   {
      inputActions.Player.Enable();
   }

   private void OnDisable()
   {
      inputActions.Player.Disable();
   }

   public void OnJump(InputAction.CallbackContext context)
   {
      if (context.performed)
      {
         Debug.Log("Jump action performed");
         // Add your jump logic here
      }
   }
}