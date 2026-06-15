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
        protected override bool RapidAttack => gunData.IsAutomatic; //TODO: Implement rapid fire
        private int _currentAmmo;
        private WaitForSeconds _waitForSecond;

        protected override void PerformAttack()
        {
            var projectile = ProjectilePool.Instance.GetProjectile();
            projectile.transform.position = firePoint.position;
            projectile.transform.forward = firePoint.forward;
            projectile.Fire(gunData.Damage, gunData.EffectiveDistance, TargetLayers);
            _currentAmmo--;
            Debug.LogError($"Fire: {_currentAmmo}/{gunData.AmmoCapacity}");
        }
        
        protected override bool CanAttack(out FailedAttackReason failedAttackReason)
        {
            var canAttack = base.CanAttack(out failedAttackReason);
            if (!canAttack && failedAttackReason == FailedAttackReason.Cooldown)
                return false;

            if (_currentAmmo == 0)
            {
                failedAttackReason = FailedAttackReason.NoAmmo;
                return false;
            }

            return canAttack;
        }
        
        public override void ShowDamageAreaPreview(bool isVisible)
        {
            weaponAimLineDrawer.SetActive(isVisible);
        }

        public override void OnEquipped()
        {
            _currentAmmo = gunData.AmmoCapacity;
            weaponAimLineDrawer.Configure(gunData.EffectiveDistance);
        }

        public override void OnUnequipped()
        {
            weaponAimLineDrawer.SetActive(false);
        }

        public override void OnDropped()
        {
            weaponAimLineDrawer.SetActive(false);
        }

        public void Reload()
        {
            _currentAmmo = gunData.AmmoCapacity;
        }
    }
}