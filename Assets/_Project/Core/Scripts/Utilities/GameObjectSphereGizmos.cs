using UnityEngine;

namespace TwinStickShooter.Core.Utilities
{
    public class GameObjectSphereGizmos : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private Color gizmosColor;
        [SerializeField] private bool isWireView;

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;
            if (isWireView)
            {
                Gizmos.DrawWireSphere(transform.position, radius);
                return;
            }

            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}