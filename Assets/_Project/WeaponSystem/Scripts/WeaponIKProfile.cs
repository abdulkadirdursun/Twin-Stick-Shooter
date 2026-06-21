using System;
using TwinStickShooter.Core;
using UnityEngine;

namespace TwinStickShooter.WeaponSystem
{
    [Serializable]
    public class WeaponIKProfile
    {
        [Header("Hands")]
        [SerializeField, Tooltip("When true the off-hand IK constraint is activated for this weapon.")] private bool useSupportHand;
        [SerializeField] private Vector3 aimOffset;
        [SerializeField] private Axis aimAxis = Axis.X;
        [SerializeField] private Axis upAxis = Axis.Z;
        [SerializeField] private float blendInTime = 0.15f;
        [Header("Spine")]
        [SerializeField] private Vector3 spineOffset;

        public bool UseSupportHand => useSupportHand;
        public Vector3 AimOffset => aimOffset;
        public Axis AimAxis => aimAxis;
        public Axis UpAxis => upAxis;
        public float BlendInTime => blendInTime;
        public Vector3 SpineOffset => spineOffset;
    }
}