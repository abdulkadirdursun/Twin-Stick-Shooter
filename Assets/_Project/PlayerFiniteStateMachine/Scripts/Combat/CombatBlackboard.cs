using TwinStickShooter.AnimationSystem;
using TwinStickShooter.WeaponSlotSystem;

namespace TwinStickShooter.PlayerFiniteStateMachine.Combat
{
    public class CombatBlackboard
    {
        #region Constructor

        public CombatBlackboard(AnimationController animationController,PlayerWeaponSlots playerWeaponSlots)
        {
            AnimationController = animationController;
            WeaponSlots = playerWeaponSlots;
        }

        #endregion

        public AnimationController AnimationController { get; }
        public PlayerWeaponSlots WeaponSlots { get; }
    }
}