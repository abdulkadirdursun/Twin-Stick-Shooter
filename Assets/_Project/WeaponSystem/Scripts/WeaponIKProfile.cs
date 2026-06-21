using System;
using TwinStickShooter.Core;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    [Serializable]
    public class WeaponIKProfile
    {
        [SerializeField, Tooltip("When true the off-hand IK constraint is activated for this weapon.")] private bool useSupportHand;
        [SerializeField] private Vector3 aimOffset;
        [SerializeField] private Vector2 aimLimits = new Vector2(-30, 30);
        [SerializeField] private Axis aimAxis = Axis.X;
        [SerializeField] private Axis upAxis = Axis.Z;
        [SerializeField, Range(0f, 1f)] private float aimWeight = 1f;
        [SerializeField, Range(0f, 1f)] private float supportHandWeight = 1f;
        [SerializeField, Range(0f, 1f)] private float hintWeight = 1f;
        [SerializeField] private float blendInTime = 0.15f;

        public bool UseSupportHand => useSupportHand;
        public Vector3 AimOffset => aimOffset;
        public Vector2 AimLimits => aimLimits;
        public Axis AimAxis => aimAxis;
        public Axis UpAxis => upAxis;
        public float AimWeight => aimWeight;
        public float SupportHandWeight => supportHandWeight;
        public float HintWeight => hintWeight;
        public float BlendInTime => blendInTime;
    }
}