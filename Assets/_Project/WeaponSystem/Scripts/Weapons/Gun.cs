using System.Collections;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class Gun : BaseWeapon
    {
        [SerializeField] private GunData gunData;
        protected override float AttackRate => gunData.AttackRate;
        protected override bool RapidAttack => gunData.IsAutomatic;
        private int _currentAmmo;
        private bool _reloading;
        private WaitForSeconds _waitForSecond;
        protected bool HasAmmo => _currentAmmo > 0;

        public override void OnEquipped()
        {
            _currentAmmo = gunData.AmmoCapacity;
            _waitForSecond ??= new WaitForSeconds(gunData.ReloadTime);
        }

        public override bool TryToAttack(out FailedAttackReason failedAttackReason)
        {
            if (!HasAmmo)
            {
                failedAttackReason = FailedAttackReason.NoAmmo;
                return false;
            }

            return base.TryToAttack(out failedAttackReason);
        }

        public void Reload()
        {
            if (_reloading) return;
            StartCoroutine(ReloadCoroutine());
        }

        protected override void Attack()
        {
            //Fire projectile
            _currentAmmo--;
            if (!HasAmmo)
                Reload();
        }

        private IEnumerator ReloadCoroutine()
        {
            _reloading = true;
            yield return _waitForSecond;
            _currentAmmo = gunData.AmmoCapacity;
            _reloading = false;
        }
    }
}