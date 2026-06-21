using PrimeTween;
using TwinStickShooter.WeaponSystem;
using UnityEngine;

namespace TwinStickShooter.AnimationSystem
{
    public class WeaponIKBinder : MonoBehaviour
    {
        [SerializeField] private WeaponController weaponController;
        [SerializeField] private IKRigController ikRigController;
        [SerializeField] private WeaponIKDriver weaponIKDriver;
        [SerializeField, Tooltip("Seconds to ramp constraint weights to zero on weapon unequip.")] private float unequipBlendTime = 0.1f;

        private Tween _blend;
        private float _targetSupport;
        private float _aimWeight;

        private void OnWeaponEquipped(BaseWeapon weapon, BaseWeaponData data)
        {
            CancelActiveBlend();
            var profile = data.IKProfile;
            ikRigController.ApplyAimProfile(
                profile.AimOffset,
                profile.AimAxis,
                profile.UpAxis);
            ikRigController.ApplySpineProfile(profile.SpineOffset);

            ikRigController.SetActiveSupportHand(profile.UseSupportHand);

            if (profile.UseSupportHand)
            {
                weaponIKDriver.SetWeaponRigPoints(weapon.RigPoints);
            }
            else
            {
                weaponIKDriver.Clear();
            }

            _targetSupport = profile.UseSupportHand ? 1f : 0f;
            _blend = Tween.Custom(0f, 1f, profile.BlendInTime, BlendWeights);
        }

        private void OnWeaponUnequipped()
        {
            weaponIKDriver.Clear();
            CancelActiveBlend();
            _targetSupport = 0f;
            _aimWeight = 0f;
            _blend = Tween.Custom(0f, 1f, unequipBlendTime, BlendWeights);
        }

        private void CancelActiveBlend()
        {
            _blend.Stop();
        }

        void BlendWeights(float time)
        {
            var aimConstraintWeight = Mathf.Lerp(0f, _aimWeight, time);
            var supportConstraintWeight = Mathf.Lerp(0f, _targetSupport, time);
        }

        #region MonoBehaviour Methods

        private void OnEnable()
        {
            weaponController.WeaponEquipped += OnWeaponEquipped;
            weaponController.WeaponUnequipped += OnWeaponUnequipped;
        }

        private void OnDisable()
        {
            weaponController.WeaponEquipped -= OnWeaponEquipped;
            weaponController.WeaponUnequipped -= OnWeaponUnequipped;
        }

        #endregion
    }
}