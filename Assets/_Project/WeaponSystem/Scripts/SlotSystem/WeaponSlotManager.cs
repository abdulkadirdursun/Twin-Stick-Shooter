using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem.SlotSystem
{
    public class WeaponSlotManager : MonoBehaviour
    {
        [SerializeField] private PlayerWeaponSlotsData playerWeaponSlotsData;
        [SerializeField] private SelectedPlayerWeaponData selectedPlayerWeaponData;
        [SerializeField] private GameplayInputs gameplayInputs;

        private void OnSlotSelected(int slotNumber)
        {
            var index = slotNumber - 1;
            if (!playerWeaponSlotsData.TryGetWeaponSlotData(index, out var weaponSlot)) return;
            selectedPlayerWeaponData.SetSelectedWeaponData(weaponSlot.WeaponData);
        }

        #region MonoBehaviour Methods

        protected void Awake()
        {
            playerWeaponSlotsData.Initialize();
        }

        private void OnEnable()
        {
            gameplayInputs.WeaponSlotSelected += OnSlotSelected;
        }

        private void OnDisable()
        {
            gameplayInputs.WeaponSlotSelected -= OnSlotSelected;
        }

        #endregion
    }
}