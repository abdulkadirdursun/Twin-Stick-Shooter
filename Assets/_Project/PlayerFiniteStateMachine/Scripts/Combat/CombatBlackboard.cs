using AKD.AnimationEvents;
using TwinStickShooter.AnimationSystem;
using TwinStickShooter.InputSystem;
using TwinStickShooter.WeaponSystem.SlotSystem;

namespace TwinStickShooter.PlayerFiniteStateMachine.Combat
{
    public class CombatBlackboard
    {
        #region Constructor

        public CombatBlackboard(AnimationController animationController,PlayerWeaponSlots playerWeaponSlots, AnimationEventDispatcher animationEventDispatcher, GameplayInputs gameplayInputs)
        {
            AnimationController = animationController;
            WeaponSlots = playerWeaponSlots;
            AnimationEventDispatcher = animationEventDispatcher;
            GameplayInputs = gameplayInputs;
        }

        #endregion

        public AnimationController AnimationController { get; }
        public PlayerWeaponSlots WeaponSlots { get; }
        public GameplayInputs GameplayInputs { get; }
        
        public AnimationEventDispatcher AnimationEventDispatcher { get; }
    }
}