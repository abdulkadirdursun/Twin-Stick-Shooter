using System.Collections;
using AKD.Toolkit.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace TwinStickShooter.WeaponSlotSystem.UI
{
    public class SlotView : MonoBehaviour
    {
        [SerializeField] private int slotIndex;
        [Header("UI Elements")]
        [SerializeField] private CanvasGroup iconCanvasGroup;
        [SerializeField] private Image iconImage;

        private WeaponSlot _slot;

        private void SetView()
        {
            if (!_slot.WeaponData)
            {
                ResetView();
                return;
            }

            iconImage.sprite = _slot.WeaponData.WeaponIcon;
            iconCanvasGroup.SetActive(true, false);
        }

        private void ResetView()
        {
            iconCanvasGroup.SetActive(false, false);
        }

        #region MonoBehaviour Methods

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerWeaponSlots.IsInstanceExist);
            _slot = PlayerWeaponSlots.Instance.GetWeaponSlot(slotIndex);
            if (_slot == null)
            {
                gameObject.SetActive(false);
                Debug.LogError($"Player Slot Index {slotIndex} is not available!!");
                yield break;
            }

            SetView();
        }

        private void OnEnable()
        {
            if (_slot == null) return;
            _slot.OnSlotChanged += SetView;
        }

        private void OnDisable()
        {
            if (_slot == null) return;
            _slot.OnSlotChanged -= SetView;
        }

        #endregion
    }
}