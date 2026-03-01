using TwinStickShooter.AnimationSystem.Enums;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    public abstract class BaseWeaponData : ScriptableObject
    {
        [SerializeField] private string weaponName = "UnnamedWeapon";
        [SerializeField] private Sprite weaponIcon;
        [SerializeField] private float attackRate = 0.35f;
        [SerializeField] private float damage = 1;
        [Header("Animator")]
        [SerializeField] private AnimatorLayer animatorLayer;
        [Header("Prefabs")]
        [SerializeField] private GameObject weaponPreviewPrefab;
        [SerializeField] private BaseWeapon weaponPrefab;
        [Header("Placement Info")]
        [SerializeField] private Vector3 holdPosition;
        [SerializeField] private Vector3 holdRotation;

        public string WeaponName => weaponName;
        public Sprite WeaponIcon => weaponIcon;
        public float AttackRate => attackRate;
        public float Damage => damage;

        public AnimatorLayer AnimatorLayer => animatorLayer;

        public GameObject WeaponPreviewPrefab => weaponPreviewPrefab;
        public BaseWeapon WeaponPrefab => weaponPrefab;
        public Vector3 HoldPosition => holdPosition;
        public Vector3 HoldRotation => holdRotation;
    }
}