using TwinStickShooter.InputSystem;
using TwinStickShooter.WeaponSystem;
using UnityEngine;

namespace TwinStickShooter.PlayerFiniteStateMachine.Combat
{
    public class AimedState : AbstractCombatState
    {
        #region Constructor

        public AimedState(CombatStateMachine stateMachine, CombatBlackboard blackboard) : base(stateMachine, blackboard)
        {
        }

        #endregion

        private static readonly int AimId = Animator.StringToHash("Aim");
        private static readonly int AttackId = Animator.StringToHash("Attack");

        private bool _isAttacking;

        public override void StateEnter()
        {
            Blackboard.AnimationController.SetBool(AimId, true);
            PlayerInputs.OnStopAiming += ChangeToIdleState;
            PlayerInputs.OnAttackPressed += OnAttackPressed;
            PlayerInputs.OnAttackReleased += OnAttackReleased;
        }

        public override void StateUpdate()
        {
            if (!_isAttacking || !Blackboard.WeaponSlots.ActiveSlot?.Weapon) return;
            var weapon = Blackboard.WeaponSlots.ActiveSlot.Weapon;
            if (!weapon.CanAttack(out var failedAttackReason))
            {
                if (failedAttackReason == FailedAttackReason.NoAmmo)
                {
                    StateMachine.ChangeState<ReloadState>();
                }
                return;
            }

            Blackboard.AnimationController.SetTrigger(AttackId);
        }

        public override void StateExit()
        {
            Blackboard.AnimationController.SetBool(AimId, false);
            PlayerInputs.OnStopAiming -= ChangeToIdleState;
            PlayerInputs.OnAttackPressed -= OnAttackPressed;
            PlayerInputs.OnAttackReleased -= OnAttackReleased;
            _isAttacking = false;
        }

        private void ChangeToIdleState()
        {
            StateMachine.ChangeState<IdleState>();
        }

        private void OnAttackPressed()
        {
            _isAttacking = true;
            Blackboard.WeaponSlots.ActiveSlot?.Weapon?.AttackPressed();
        }

        private void OnAttackReleased()
        {
            _isAttacking = false;
            Blackboard.WeaponSlots.ActiveSlot?.Weapon?.AttackReleased();
        }
    }
}