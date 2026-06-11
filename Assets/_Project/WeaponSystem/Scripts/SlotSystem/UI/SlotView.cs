using AKD.Toolkit.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace TwinStickShooter.WeaponSystem.SlotSystem
{
    public class SlotView : MonoBehaviour
    {
        [SerializeField] private PlayerWeaponSlotsData playerWeaponSlotsData;
        [SerializeField] private int slotIndex;
        [Header("UI Elements")]
        [SerializeField] private CanvasGroup iconCanvasGroup;
        [SerializeField] private Image iconImage;

        private WeaponSlot _slot;

        private void UpdateView()
        {
            if (!_slot.WeaponData)
            {
                iconCanvasGroup.SetActive(false, false);
                return;
            }

            iconImage.sprite = _slot.WeaponData.WeaponIcon;
            iconCanvasGroup.SetActive(true, false);
        }

        #region MonoBehaviour Methods

        private void Start()
        {
            if (!playerWeaponSlotsData.TryGetWeaponSlotData(slotIndex, out _slot))
            {
                Destroy(gameObject);
                return;
            }

            _slot.OnSlotChanged += UpdateView;
            UpdateView();
        }

        private void OnDestroy()
        {
            if (_slot == null) return;
            _slot.OnSlotChanged -= UpdateView;
        }

        #endregion
    }
}