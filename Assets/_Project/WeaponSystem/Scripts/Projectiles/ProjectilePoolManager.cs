using UnityEngine;

namespace TwinStickShooter.WeaponSystem.Projectiles
{
    public class ProjectilePoolManager : MonoBehaviour
    {
        [SerializeField] private ProjectilePoolService[] pools;

        #region MonoBehaviour Methods

        private void Awake()
        {
            foreach (var pool in pools)
            {
                pool.Initialize(transform);
            }
        }

        #endregion
    }
}