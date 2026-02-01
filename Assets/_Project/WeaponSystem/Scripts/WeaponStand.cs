using PrimeTween;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class WeaponStand : MonoBehaviour
    {
        [SerializeField] private BaseWeaponData weaponData;
        [SerializeField] private Transform previewParent;
        [Header("Animation")]
        [SerializeField] private Ease animationEase = Ease.Linear;
        [SerializeField] private float animationTime = 4f;

        private Tween _previewRotationTween;

        #region MonoBehaviour Methods

        private void Awake()
        {
            Instantiate(weaponData.WeaponPreviewPrefab, previewParent);
            _previewRotationTween = Tween.LocalEulerAngles(previewParent, Vector3.zero, Vector3.up * 360f, animationTime, animationEase, -1);
        }

        private void OnDestroy()
        {
            if (_previewRotationTween.isAlive)
                _previewRotationTween.Stop();
        }

        #endregion
    }
}