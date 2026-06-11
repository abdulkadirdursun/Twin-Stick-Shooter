using AKD.AnimationEvents;
using TwinStickShooter.AnimationSystem;
using TwinStickShooter.InputSystem;
using TwinStickShooter.MovementSystem;
using TwinStickShooter.PlayerFiniteStateMachine.Combat;
using TwinStickShooter.PlayerFiniteStateMachine.Locomotion;
using TwinStickShooter.WeaponSystem.SlotSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine
{
    public class PlayerBrain : MonoBehaviour
    {
        [SerializeField] private GameplayInputs gameplayInputs;
        [Header("Common Components")]
        [SerializeField] private AnimationController animationController;
        [Header("Locomotion Components")]
        [SerializeField] private MovementController movementController;
        [SerializeField] private IKRigController ikRigController;
        [SerializeField] private Transform aimTarget;
        [Header("Combat Components")]
        [SerializeField] private PlayerWeaponSlots playerWeaponSlots;
        [SerializeField] private AnimationEventDispatcher animationEventDispatcher;

        private LocomotionStateMachine _locomotionStateMachine;
        private CombatStateMachine _combatStateMachine;

        private void UpdateLocomotionState()
        {
            if (_locomotionStateMachine == null)
            {
                Debug.LogError("Locomotion State Machine is null!!!");
                return;
            }

            _locomotionStateMachine.CallStateUpdate();
        }

        private void UpdateCombatState()
        {
            if (_combatStateMachine == null)
            {
                Debug.LogError("Combat State Machine is null!!!");
                return;
            }

            _combatStateMachine.CallStateUpdate();
        }

        #region MonoBehaviour Methods

        private void Awake()
        {
            var locomotionBlackboard = new LocomotionBlackboard(movementController, animationController, ikRigController, aimTarget, gameplayInputs);
            _locomotionStateMachine = new LocomotionStateMachine(locomotionBlackboard);
            var combatBlackboard = new CombatBlackboard(animationController, playerWeaponSlots, animationEventDispatcher, gameplayInputs);
            _combatStateMachine = new CombatStateMachine(combatBlackboard);
        }

        private void Update()
        {
            UpdateLocomotionState();
            UpdateCombatState();
        }

        #endregion
    }
}