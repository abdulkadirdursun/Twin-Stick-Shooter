using UnityEngine;

namespace TwinStickShooter.AnimationSystem
{
    public class IKAimTargetPlacer : MonoBehaviour
    {
        [SerializeField] private Transform aimTarget;
        [SerializeField] private float distanceFromBody = 10f;

        public void SetActive(bool value)
        {
        }

        public void SetDirection(Vector3 lookDirection)
        {
            var position = lookDirection * distanceFromBody;
            position.y = aimTarget.position.y;
            aimTarget.position = position;
        }
    }
}