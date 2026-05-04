using TwinStickShooter.AnimationSystem;
using TwinStickShooter.MovementSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine.Locomotion
{
    public class LocomotionBlackboard
    {
        #region Constructor

        public LocomotionBlackboard(
            MovementController movementController, 
            AnimationController animationController, 
            IKRigController ikRigController,
            Transform aimTarget)
        {
            MovementController = movementController;
            AnimationController = animationController;
            IKRigController = ikRigController;
            AimTarget = aimTarget;
        }

        #endregion

        public MovementController MovementController { get; }
        public AnimationController AnimationController { get; }
        public IKRigController IKRigController { get; }
        public Transform AimTarget { get; }

        public Vector3 MoveDirection { get; set; }
        public Vector3 LookDirection { get; set; }
    }
}