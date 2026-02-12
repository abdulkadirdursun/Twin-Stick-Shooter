using TwinStickShooter.AnimationSystem;
using TwinStickShooter.MovementSystem;

namespace TwinStickShooter.PlayerFiniteStateMachine.Locomotion
{
    public class LocomotionBlackboard
    {
        #region Constructor

        public LocomotionBlackboard(MovementController movementController, AnimationController animationController)
        {
            MovementController = movementController;
            AnimationController = animationController;
        }

        #endregion
        
        public MovementController MovementController { get; }
        public AnimationController AnimationController { get; }
    }
}