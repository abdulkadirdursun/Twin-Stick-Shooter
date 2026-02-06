using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    [CreateAssetMenu(fileName = "NewGunData", menuName = "Twin Stick Shooter/Weapon System/New Gun Data")]
    public class GunData : BaseWeaponData
    {
        [Header("Gun")]
        [SerializeField] private bool isAutomatic;
        [SerializeField] private int ammoCapacity = 7;
        [SerializeField] private float reloadTime = 1.5f;
        
        public bool IsAutomatic => isAutomatic;
        public int AmmoCapacity => ammoCapacity;
        public float ReloadTime => reloadTime;
    }
}