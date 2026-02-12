using TwinStickShooter.AnimationSystem;

namespace TwinStickShooter.PlayerFiniteStateMachine.Combat
{
    public class CombatBlackboard
    {
        #region Constructor

        public CombatBlackboard(AnimationController animationController)
        {
            AnimationController = animationController;
        }

        #endregion

        public AnimationController AnimationController { get; }
    }
}