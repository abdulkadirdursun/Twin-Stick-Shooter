using UnityEngine;

namespace TwinStickShooter.AnimationSystem
{
    public class WeaponIKDriver : MonoBehaviour
    {
        [SerializeField] private Transform supportHandTarget;
        [SerializeField] private Transform supportHandHint;

        private WeaponRigPoints _weaponRigPoints;

        public void SetWeaponRigPoints(WeaponRigPoints weaponRigPoints)
        {
            _weaponRigPoints = weaponRigPoints;
        }

        public void Clear()
        {
            _weaponRigPoints = null;
        }

        #region MonoBehaviour Methods

        private void Update()
        {
            if (_weaponRigPoints == null) return;
            supportHandTarget.SetPositionAndRotation(
                _weaponRigPoints.SupportHandGrip.position,
                _weaponRigPoints.SupportHandGrip.rotation);

            supportHandHint.SetPositionAndRotation(
                _weaponRigPoints.SupportHint.position,
                _weaponRigPoints.SupportHint.rotation);
        }

        #endregion
    }
}