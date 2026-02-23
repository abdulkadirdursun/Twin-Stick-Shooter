using AKD.Toolkit.ObjectPooling;
using AKD.Toolkit.Singleton;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem.Projectiles
{
    public class ProjectilePool : Singleton<ProjectilePool>
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private int startPoolSize = 5;
        [SerializeField] private int maxPoolSize = 35;

        private ObjectPool<Projectile> _projectilePool;

        public Projectile GetProjectile() => _projectilePool.Get();
        public void ReleaseProjectile(Projectile projectile) => _projectilePool.Release(projectile);

        private void Initialize()
        {
            _projectilePool = new ObjectPool<Projectile>(projectilePrefab, startPoolSize, maxPoolSize, transform, OnCreate, OnGet, OnRelease);
        }

        #region Pool Methods

        private void OnCreate(Projectile projectile, ObjectPool<Projectile> pool)
        {
            projectile.gameObject.SetActive(false);
        }

        private void OnGet(Projectile projectile)
        {
            projectile.gameObject.SetActive(true);
        }

        private void OnRelease(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
        }

        #endregion

        #region MonoBehaviour Methods

        protected override void OnAwake()
        {
            base.OnAwake();
            Initialize();
        }

        #endregion
    }
}