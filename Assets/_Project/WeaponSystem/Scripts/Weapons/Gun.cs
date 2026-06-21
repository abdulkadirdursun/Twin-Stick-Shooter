using AKD.AnimationEvents;
using TwinStickShooter.WeaponSystem.Projectiles;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class Gun : BaseWeapon
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private WeaponAimLineDrawer weaponAimLineDrawer;
        [SerializeField] private GunData gunData;
        [SerializeField] private ProjectilePoolService projectilePoolService;
        protected override float AttackRate => gunData.AttackRate;
        protected override bool RapidAttack => gunData.IsAutomatic; //TODO: Implement rapid fire
        private int _currentAmmo;

        private readonly int _reloadAnimationId = Animator.StringToHash("Reload");

        protected override void PerformAttack()
        {
            var projectile = projectilePoolService.Get();
            projectile.transform.position = firePoint.position;
            projectile.transform.forward = firePoint.forward;
            projectile.Fire(gunData.Damage, gunData.EffectiveDistance, TargetLayers);
            _currentAmmo--;
            Debug.LogError($"Fire: {_currentAmmo}/{gunData.AmmoCapacity}");
        }

        protected override bool CanAttack()
        {
            if (!base.CanAttack() || _currentAmmo == 0) return false;

            return true;
        }

        public override void ShowDamageAreaPreview(bool isVisible)
        {
            weaponAimLineDrawer.SetActive(isVisible);
        }

        protected override void OnEquipped()
        {
            _currentAmmo = gunData.AmmoCapacity;
            weaponAimLineDrawer.Configure(gunData.EffectiveDistance);
        }

        protected override void OnUnequipped()
        {
            weaponAimLineDrawer.SetActive(false);
        }

        protected override void OnDropped()
        {
            weaponAimLineDrawer.SetActive(false);
        }

        protected override void SubscribeAnimationEvents()
        {
            AnimationEventDispatcher.Register(AnimationEventScope.State(_reloadAnimationId), AnimationEventType.OnComplete, OnReloadAnimationComplete);
        }

        protected override void UnsubscribeAnimationEvents()
        {
            AnimationEventDispatcher.Unregister(AnimationEventScope.State(_reloadAnimationId), AnimationEventType.OnComplete, OnReloadAnimationComplete);
        }

        private void OnReloadAnimationComplete(AnimationEventContext context)
        {
            _currentAmmo = gunData.AmmoCapacity;
        }
    }
}