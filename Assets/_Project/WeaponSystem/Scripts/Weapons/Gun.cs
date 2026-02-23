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

        public  void Shoot()
        {
            //Fire projectile
            _currentAmmo--;
            Debug.LogError($"Fire: {_currentAmmo}/{gunData.AmmoCapacity}");
        }

        public override void OnEquipped()
        {
            _currentAmmo = gunData.AmmoCapacity;
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