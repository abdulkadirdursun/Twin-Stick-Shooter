using UnityEngine;

namespace TwinStickShooter.AnimationSystem
{
    public class WeaponRigPoints : MonoBehaviour
    {
        [SerializeField] private Transform mainHandGrip;
        [SerializeField] private Transform supportHandGrip;
        [SerializeField] private Transform supportHint;

        public Transform MainHandGrip => mainHandGrip;
        public Transform SupportHandGrip => supportHandGrip;
        public Transform SupportHint => supportHint;
    }
}