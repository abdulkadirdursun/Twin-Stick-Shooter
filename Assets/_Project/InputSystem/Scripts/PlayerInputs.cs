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
        [SerializeField] private GameplayInputs gameplayInputs;
        private PlayerInputActions _playerInputActions;
        private InputControlScheme _inputControlScheme;

        private void Initialize()
        {
            _playerInputActions = new PlayerInputActions();
            _inputControlScheme = new InputControlScheme(_playerInputActions);
            //Gameplay
            gameplayInputs.Initialize(_playerInputActions.Gameplay);
        }


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