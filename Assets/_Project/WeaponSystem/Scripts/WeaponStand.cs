using PrimeTween;
using TwinStickShooter.InputSystem.View;
using TwinStickShooter.InteractionSystem;
using TwinStickShooter.WeaponSlotSystem;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public class WeaponStand : MonoBehaviour, IInteractable
    {
        [Header("Showcased Weapon")]
        [SerializeField] private BaseWeaponData weaponData;
        [SerializeField] private Transform previewParent;
        [Header("Animation")]
        [SerializeField] private Ease animationEase = Ease.Linear;
        [SerializeField] private float animationTime = 4f;
        [Header("Button View")]
        [SerializeField] private SpriteIconChanger spriteIconChanger; 

        private Tween _previewRotationTween;

        public Vector3 Position => transform.position;

        public void Interact()
        {
            PlayerWeaponSlots.Instance.TryToEquip(weaponData);
        }

        public void ShowInteractableIndicator()
        {
            spriteIconChanger.Show();
        }

        public void HideInteractableIndicator()
        {
            spriteIconChanger.Hide();
        }

        #region MonoBehaviour Methods

        private void Awake()
        {
            Instantiate(weaponData.WeaponPreviewPrefab, previewParent);
            _previewRotationTween = Tween.LocalEulerAngles(previewParent, Vector3.zero, Vector3.up * 360f, animationTime, animationEase, -1);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out InteractionManager interactionManager)) return;
            interactionManager.AddInteractable(this);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out InteractionManager interactionManager)) return;
            interactionManager.RemoveInteractable(this);
        }

        private void OnDestroy()
        {
            if (_previewRotationTween.isAlive)
                _previewRotationTween.Stop();
        }

        #endregion
    }
}