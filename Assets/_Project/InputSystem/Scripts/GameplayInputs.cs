using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TwinStickShooter.InputSystem
{
    [CreateAssetMenu(fileName = "GameplayInputs", menuName = "Twin Stick Shooter/Input System/Gameplay Inputs")]
    public class GameplayInputs : ScriptableObject
    {
        private PlayerInputActions.GameplayActions _gameplayActions;

        public event Action<Vector2> MoveInput;
        public event Action StartAiming;
        public event Action StopAiming;
        public event Action<Vector2> LookTargetMovement;
        public event Action<Vector2> CursorPositionInput;
        public event Action Interact;
        public event Action<int> WeaponSlotSelected;
        public event Action AttackPressed;
        public event Action AttackReleased;

        public event Action Reload;

        public void Initialize(PlayerInputActions.GameplayActions gameplayActions)
        {
            _gameplayActions = gameplayActions;

            _gameplayActions.Movement.performed += ReadMovementInput;
            _gameplayActions.AimTrigger.performed += OnMouseRightClickPerformed;
            _gameplayActions.AimTrigger.canceled += OnMouseRightClickCancelled;
            _gameplayActions.LookTargetMovement.performed += ReadLookTargetMovement;
            _gameplayActions.Aim.performed += ReadAim;
            _gameplayActions.Interact.performed += OnInteractButtonClicked;
            _gameplayActions.WeaponSlot1.performed += OnWeaponSlot1Selected;
            _gameplayActions.WeaponSlot2.performed += OnWeaponSlot2Selected;
            _gameplayActions.WeaponSlot3.performed += OnWeaponSlot3Selected;
            _gameplayActions.Attack.performed += AttackInputPerformed;
            _gameplayActions.Attack.canceled += AttackInputCancelled;
            _gameplayActions.Reload.performed += ReloadButtonClick;
            _gameplayActions.Enable();
        }

        private void ReadMovementInput(InputAction.CallbackContext context)
        {
            var movementInput = context.ReadValue<Vector2>();
            MoveInput?.Invoke(movementInput);
        }

        private void OnMouseRightClickPerformed(InputAction.CallbackContext context)
        {
            StartAiming?.Invoke();
        }

        private void OnMouseRightClickCancelled(InputAction.CallbackContext context)
        {
            StopAiming?.Invoke();
        }

        private void ReadLookTargetMovement(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            LookTargetMovement?.Invoke(value);
        }

        private void ReadAim(InputAction.CallbackContext context)
        {
            var aimPosition = context.ReadValue<Vector2>();
            CursorPositionInput?.Invoke(aimPosition);
        }

        private void OnInteractButtonClicked(InputAction.CallbackContext context)
        {
            Interact?.Invoke();
        }

        private void OnWeaponSlot1Selected(InputAction.CallbackContext context)
        {
            WeaponSlotSelected?.Invoke(1);
        }

        private void OnWeaponSlot2Selected(InputAction.CallbackContext context)
        {
            WeaponSlotSelected?.Invoke(2);
        }

        private void OnWeaponSlot3Selected(InputAction.CallbackContext context)
        {
            WeaponSlotSelected?.Invoke(3);
        }

        private void AttackInputPerformed(InputAction.CallbackContext context)
        {
            AttackPressed?.Invoke();
        }

        private void AttackInputCancelled(InputAction.CallbackContext context)
        {
            AttackReleased?.Invoke();
        }

        private void ReloadButtonClick(InputAction.CallbackContext context)
        {
            Reload?.Invoke();
        }
    }
}