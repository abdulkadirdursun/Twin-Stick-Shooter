using UnityEngine;

namespace TwinStickShooter
{
    public class GameObjectSphereGizmos : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private Color gizmosColor;
        [SerializeField] private bool isWireView;

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;
            switch (isWireView)
            {
                case true:
                    Gizmos.DrawWireSphere(transform.position, radius);
                    break;
                case false:
                    Gizmos.DrawSphere(transform.position, radius);
                    break;
            }
        }
    }
}