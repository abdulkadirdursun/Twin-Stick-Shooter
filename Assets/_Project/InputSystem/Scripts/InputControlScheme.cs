using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace TwinStickShooter.InputSystem
{
    public class InputControlScheme : IDisposable
    {
        #region Contructor

        public InputControlScheme(PlayerInputActions playerInputActions)
        {
            CreateUserWithAvailableDevices();
            _inputUser.AssociateActionsWithUser(playerInputActions);
            ++InputUser.listenForUnpairedDeviceActivity;
            InputUser.onUnpairedDeviceUsed += HandleUnpairedDeviceUsed;
        }

        #endregion

        #region Static Fields

        public static ControlSchemeType CurrentControlType { get; private set; } = ControlSchemeType.Undefined;
        public static event Action<ControlSchemeType> OnControlSchemeTypeChanged;

        #endregion

        private InputUser _inputUser;
        private bool _inputUserCreated;

        private void CreateUserWithAvailableDevices()
        {
            PairWithKeyboardAndMouse();
            if (!_inputUserCreated)
                PairWithGamepad();
            if (_inputUserCreated) return;
            _inputUser = InputUser.CreateUserWithoutPairedDevices();
            CurrentControlType = ControlSchemeType.Undefined;
        }

        private void PairWithKeyboardAndMouse()
        {
            var paired = false;
            if (Keyboard.current != null)
            {
                PairWithDevice(Keyboard.current);
                paired = true;
            }

            if (Mouse.current != null)
            {
                PairWithDevice(Mouse.current);
                paired = true;
            }

            if (paired)
                CurrentControlType = ControlSchemeType.KeyboardMouse;
        }

        private void PairWithGamepad()
        {
            if (Gamepad.current == null) return;

            PairWithDevice(Gamepad.current);
            CurrentControlType = ControlSchemeType.Gamepad;
        }

        private void PairWithDevice(InputDevice device)
        {
            if (!_inputUserCreated)
            {
                _inputUser = InputUser.PerformPairingWithDevice(device);
                return;
            }

            InputUser.PerformPairingWithDevice(device, _inputUser);
        }

        private void HandleUnpairedDeviceUsed(InputControl control, InputEventPtr eventPtr)
        {
            var device = control.device;
            var newControlType = GetControlTypeFromDevice(device);
            if (newControlType == CurrentControlType)
            {
                PairWithDevice(device);
                return;
            }

            _inputUser.UnpairDevices();
            InputUser.PerformPairingWithDevice(device, user: _inputUser);
            CurrentControlType = newControlType;
            OnControlSchemeTypeChanged?.Invoke(CurrentControlType);
        }

        private ControlSchemeType GetControlTypeFromDevice(InputDevice device)
        {
            if (device is Gamepad)
                return ControlSchemeType.Gamepad;
            if (device is Keyboard or Mouse)
                return ControlSchemeType.KeyboardMouse;

            return ControlSchemeType.Undefined;
        }

        public void Dispose()
        {
            --InputUser.listenForUnpairedDeviceActivity;
            if (_inputUser.valid)
                _inputUser.UnpairDevicesAndRemoveUser();
            InputUser.onUnpairedDeviceUsed -= HandleUnpairedDeviceUsed;
        }
    }
}