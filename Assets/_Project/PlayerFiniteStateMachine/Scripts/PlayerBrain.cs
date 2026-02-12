using TwinStickShooter.AnimationSystem;
using TwinStickShooter.MovementSystem;
using TwinStickShooter.PlayerFiniteStateMachine.Locomotion;
using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine
{
    public class PlayerBrain : MonoBehaviour
    {
        [Header("Locomotion")]
        [SerializeField] private MovementController movementController;
        [SerializeField] private AnimationController animationController;

        private LocomotionStateMachine _locomotionStateMachine;

        #region MonoBehaviour Methods

        private void Awake()
        {
            var locomotionBlackboard = new LocomotionBlackboard(movementController, animationController);
            _locomotionStateMachine = new LocomotionStateMachine(locomotionBlackboard);
        }

        private void Update()
        {
            if (_locomotionStateMachine == null)
            {
                Debug.LogError("Locomotion State Machine is null!!!");
                return;
            }

            _locomotionStateMachine.CallStateUpdate();
        }

        #endregion
    }
}