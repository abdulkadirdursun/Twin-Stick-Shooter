using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public abstract class BaseWeaponData : ScriptableObject
    {
        [SerializeField] private string weaponName = "UnnamedWeapon";
        [SerializeField] private Sprite weaponIcon;
        [SerializeField] private float attackRate = 0.35f;
        [SerializeField] private float damage = 1;
        [Header("Prefabs")]
        [SerializeField] private GameObject weaponPreviewPrefab;
        [SerializeField] private BaseWeapon weaponPrefab;

        public string WeaponName => weaponName;
        public Sprite WeaponIcon => weaponIcon;
        public float AttackRate => attackRate;
        public float Damage => damage;

        public GameObject WeaponPreviewPrefab => weaponPreviewPrefab;
        public BaseWeapon WeaponPrefab => weaponPrefab;
    }
}