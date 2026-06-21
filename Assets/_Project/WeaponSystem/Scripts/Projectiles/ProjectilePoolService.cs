using TwinStickShooter.ObjectPooling;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem.Projectiles
{
    [CreateAssetMenu(fileName = "ProjectilePoolService", menuName = "Twin Stick Shooter/Weapon System/Projectile Pool Service")]
    public class ProjectilePoolService : ScriptableObject
    {
        [SerializeField] private Projectile prefab;
        [SerializeField] private int startPoolSize = 1;
        [SerializeField] private int maxPoolSize = 10;

        private ObjectPool<Projectile> _projectilePool;

        public Projectile Get() => _projectilePool.Get();

        public void Initialize(Transform parent)
        {
            _projectilePool = new ObjectPool<Projectile>(
                prefab,
                parent,
                startPoolSize,
                maxPoolSize,
                OnCreate);
        }

        private void OnCreate(Projectile projectile, ObjectPool<Projectile> pool)
        {
            projectile.Destroyed += pool.ReleaseRequest;
        }
    }
}