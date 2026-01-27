using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TwinStickShooter.InputSystem
{
    /*# Why am I registering to input events but not unregister
     *Player inputs object will never disable once game session start.
     *We are registering on beginning of the session and dispose the player inputs actions on the end 
     */
    public class PlayerInputs : MonoBehaviour
    {
        #region Static

        public static event Action<Vector2> OnMove;

        #endregion

        private PlayerInputActions _playerInputActions;

        private void Initialize()
        {
            _playerInputActions = new PlayerInputActions();
            //Gameplay
            _playerInputActions.Gameplay.Movement.performed += ReadMovementInput;
            
            _playerInputActions.Gameplay.Enable();
        }

        #region Gameplay Inputs

        private void ReadMovementInput(InputAction.CallbackContext context)
        {
            var movementInput = context.ReadValue<Vector2>();
            OnMove?.Invoke(movementInput);
        }

        #endregion

        #region MonoBehaviour Methods

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            _playerInputActions.Dispose();
        }

        #endregion
    }
}