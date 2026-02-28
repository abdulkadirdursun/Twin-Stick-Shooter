using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TwinStickShooter.AnimationSystem
{
    public class IKAimTargetPlacer : MonoBehaviour
    {
        [SerializeField] private Rig aimRig;
        [SerializeField] private Transform aimTarget;
        [SerializeField] private float distanceFromBody = 10f;

        public void SetActive(bool value)
        {
            aimRig.weight = value ? 1f : 0f;
        }

        public void SetDirection(Vector3 lookDirection)
        {
            var position = lookDirection * distanceFromBody;
            position.y = aimTarget.position.y;
            aimTarget.position = position;
        }
    }
}