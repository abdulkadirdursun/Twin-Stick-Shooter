using TwinStickShooter.WeaponSystem.Projectiles;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class Gun : BaseWeapon
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private WeaponAimLineDrawer weaponAimLineDrawer;
        [SerializeField] private GunData gunData;
        protected override float AttackRate => gunData.AttackRate;
        protected override bool RapidAttack => gunData.IsAutomatic;
        private int _currentAmmo;
        private bool _reloading;
        private WaitForSeconds _waitForSecond;
        protected bool HasAmmo => _currentAmmo > 0;

        public override void OnStartAim()
        {
            weaponAimLineDrawer.SetActive(true);
        }

        public override void OnStopAim()
        {
            weaponAimLineDrawer.SetActive(false);
        }

        public void Shoot()
        {
            var projectile = ProjectilePool.Instance.GetProjectile();
            projectile.transform.position = firePoint.position;
            projectile.transform.forward = firePoint.forward;
            projectile.Fire(gunData.Damage, gunData.EffectiveDistance, TargetLayers);
            _currentAmmo--;
            Debug.LogError($"Fire: {_currentAmmo}/{gunData.AmmoCapacity}");
        }

        public override void OnEquipped()
        {
            _currentAmmo = gunData.AmmoCapacity;
            weaponAimLineDrawer.Configure(gunData.EffectiveDistance);
        }

        public override void OnUnequipped()
        {
            base.OnUnequipped();
            weaponAimLineDrawer.SetActive(false);
        }

        public override bool CanAttack(out FailedAttackReason failedAttackReason)
        {
            if (!HasAmmo)
            {
                failedAttackReason = FailedAttackReason.NoAmmo;
                return false;
            }

            return base.CanAttack(out failedAttackReason);
        }

        public void Reload()
        {
            if (_reloading) return;
            _reloading = true;
            _currentAmmo = gunData.AmmoCapacity;
            _reloading = false;
        }
    }
}