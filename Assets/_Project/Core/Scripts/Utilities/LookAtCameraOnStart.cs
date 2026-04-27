using UnityEngine;

namespace TwinStickShooter.Core.Utilities
{
    public class LookAtCameraOnStart : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransformToLookAt;

        #region MonoBehaviour Methods

        private void Awake()
        {
            if (cameraTransformToLookAt) return;
            cameraTransformToLookAt = Camera.main?.transform;
        }

        private void Start()
        {
            if (!cameraTransformToLookAt) return;
            //Calculate X axis
            var lookDirection = cameraTransformToLookAt.position - transform.position;
            var targetXAsis = Quaternion.LookRotation(lookDirection).eulerAngles.x;
            
            var targetYaw = 180f;//Always look -Z direction, towards the camera
            transform.rotation = Quaternion.Euler(targetXAsis, targetYaw, 0f);
        }

        #endregion
    }
}