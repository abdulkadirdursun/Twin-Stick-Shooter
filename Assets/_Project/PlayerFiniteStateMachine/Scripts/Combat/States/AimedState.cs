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
            SetWeaponAimState(true);
            PlayerInputs.OnStopAiming += ChangeToIdleState;
            PlayerInputs.OnAttackPressed += OnAttackPressed;
            PlayerInputs.OnAttackReleased += OnAttackReleased;
            Blackboard.WeaponSlots.OnActiveSlotChanged += OnWeaponSlotChanged;
        }

        public override void StateUpdate()
        {
            if (!_isAttacking || !Blackboard.WeaponSlots.ActiveSlotHasWeapon(out var weapon)) return;
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
            SetWeaponAimState(false);
            Blackboard.WeaponSlots.OnActiveSlotChanged -= OnWeaponSlotChanged;
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
            if (!Blackboard.WeaponSlots.ActiveSlotHasWeapon(out var weapon)) return;
            weapon.AttackPressed();
        }

        private void OnAttackReleased()
        {
            _isAttacking = false;
            if (!Blackboard.WeaponSlots.ActiveSlotHasWeapon(out var weapon)) return;
            weapon.AttackReleased();
        }

        private void OnWeaponSlotChanged()
        {
            SetWeaponAimState(true);
        }

        private void SetWeaponAimState(bool value)
        {
            if (!Blackboard.WeaponSlots.ActiveSlotHasWeapon(out var weapon)) return;
            switch (value)
            {
                case true:
                    weapon.OnStartAim();
                    break;
                case false:
                    weapon.OnStopAim();
                    break;
            }
        }
    }
}