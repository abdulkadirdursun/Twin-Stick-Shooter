using UnityEngine;

namespace TwinStickShooter.MovementSystem.FSM
{
    public class PlayerMovementBlackboard
    {
        #region Constructor

        public PlayerMovementBlackboard(CharacterController characterController, float moveSpeed, float rotateSpeed)
        {
            CharacterController = characterController;
            Transform = characterController.transform;
            MoveSpeed = moveSpeed;
            RotateSpeed = rotateSpeed;

        }

        #endregion

        public CharacterController CharacterController { get; }
        public Transform Transform { get; }
        public float MoveSpeed { get; private set; }
        public float RotateSpeed { get; private set; }
    }
}