using AKD.AnimationEvents;
using TwinStickShooter.AnimationSystem;
using TwinStickShooter.InputSystem;
using TwinStickShooter.WeaponSystem;

namespace TwinStickShooter.PlayerFiniteStateMachine.Combat
{
    public class CombatBlackboard
    {
        #region Constructor

        public CombatBlackboard(
            AnimationController animationController,
            WeaponController weaponController,
            GameplayInputs gameplayInputs,
            AnimationEventDispatcher animationEventDispatcher)
        {
            AnimationController = animationController;
            WeaponController = weaponController;
            GameplayInputs = gameplayInputs;
            AnimationEventDispatcher = animationEventDispatcher;
        }

        #endregion

        public AnimationController AnimationController { get; }
        public WeaponController WeaponController { get; }
        public GameplayInputs GameplayInputs { get; }
        public AnimationEventDispatcher AnimationEventDispatcher { get; }
    }
}