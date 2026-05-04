using AKD.AnimationEvents;
using TwinStickShooter.AnimationSystem;
using TwinStickShooter.WeaponSlotSystem;

namespace TwinStickShooter.PlayerFiniteStateMachine.Combat
{
    public class CombatBlackboard
    {
        #region Constructor

        public CombatBlackboard(AnimationController animationController,PlayerWeaponSlots playerWeaponSlots, AnimationEventDispatcher animationEventDispatcher)
        {
            AnimationController = animationController;
            WeaponSlots = playerWeaponSlots;
            AnimationEventDispatcher = animationEventDispatcher;
        }

        #endregion

        public AnimationController AnimationController { get; }
        public PlayerWeaponSlots WeaponSlots { get; }
        
        public AnimationEventDispatcher AnimationEventDispatcher { get; }
    }
}