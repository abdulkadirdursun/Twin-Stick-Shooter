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
        private PlayerInputActions _playerInputActions;
        private InputControlScheme _inputControlScheme;

        private void Initialize()
        {
            _playerInputActions = new PlayerInputActions();
            _inputControlScheme = new InputControlScheme(_playerInputActions);
            //Gameplay
            _playerInputActions.Gameplay.Movement.performed += ReadMovementInput;
            _playerInputActions.Gameplay.MouseAimTrigger.performed += OnMouseRightClickPerformed;
            _playerInputActions.Gameplay.MouseAimTrigger.canceled += OnMouseRightClickCancelled;
            _playerInputActions.Gameplay.Aim.performed += ReadAim;
            _playerInputActions.Gameplay.Interact.performed += OnInteractButtonClicked;
            _playerInputActions.Gameplay.WeaponSlot1.performed += OnWeaponSlot1Selected;
            _playerInputActions.Gameplay.WeaponSlot2.performed += OnWeaponSlot2Selected;
            _playerInputActions.Gameplay.WeaponSlot3.performed += OnWeaponSlot3Selected;
            _playerInputActions.Gameplay.Attack.performed += AttackInputPerformed;
            _playerInputActions.Gameplay.Attack.canceled += AttackInputCancelled;

            _playerInputActions.Gameplay.Enable();
        }

        #region Gameplay Inputs

        public static event Action<Vector2> MoveInput;
        public static event Action OnStartAiming;
        public static event Action OnStopAiming;
        public static event Action<Vector2> AimPositionInput;
        public static event Action Interact;
        public static event Action<int> OnWeaponSlotSelected;
        public static event Action OnStartAttacking;
        public static event Action OnStopAttacking;

        private void ReadMovementInput(InputAction.CallbackContext context)
        {
            var movementInput = context.ReadValue<Vector2>();
            MoveInput?.Invoke(movementInput);
        }

        private void OnMouseRightClickPerformed(InputAction.CallbackContext context)
        {
            OnStartAiming?.Invoke();
        }

        private void OnMouseRightClickCancelled(InputAction.CallbackContext context)
        {
            OnStopAiming?.Invoke();
        }

        private void ReadAim(InputAction.CallbackContext context)
        {
            var aimPosition = context.ReadValue<Vector2>();
            if (InputControlScheme.CurrentControlType == ControlSchemeType.Gamepad)
            {
                if (aimPosition == Vector2.zero)
                    OnStopAiming?.Invoke();
                else
                    OnStartAiming?.Invoke();
            }

            AimPositionInput?.Invoke(aimPosition);
        }

        private void OnInteractButtonClicked(InputAction.CallbackContext context)
        {
            Interact?.Invoke();
        }

        private void OnWeaponSlot1Selected(InputAction.CallbackContext context)
        {
            OnWeaponSlotSelected?.Invoke(1);
        }

        private void OnWeaponSlot2Selected(InputAction.CallbackContext context)
        {
            OnWeaponSlotSelected?.Invoke(2);
        }

        private void OnWeaponSlot3Selected(InputAction.CallbackContext context)
        {
            OnWeaponSlotSelected?.Invoke(3);
        }

        private void AttackInputPerformed(InputAction.CallbackContext context)
        {
            OnStartAttacking?.Invoke();
        }

        private void AttackInputCancelled(InputAction.CallbackContext context)
        {
            OnStopAttacking?.Invoke();
        }

        #endregion

        #region MonoBehaviour Methods

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            _inputControlScheme.Dispose();
            _playerInputActions.Dispose();
        }

        #endregion
    }
}