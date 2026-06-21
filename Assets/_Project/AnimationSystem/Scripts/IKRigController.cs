using TwinStickShooter.Core;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TwinStickShooter.AnimationSystem
{
    public class IKRigController : MonoBehaviour
    {
        [Header("Rigs")]
        [SerializeField] private Rig weaponRig;
        [Header("Constraints")]
        [SerializeField] private MultiAimConstraint aimConstraint;
        [SerializeField] private MultiAimConstraint spineConstraint;
        [SerializeField] private TwoBoneIKConstraint supportHandConstraint;

        public void SetActive(bool value)
        {
            weaponRig.weight = value ? 1f : 0f;
        }

        public void SetActiveSupportHand(bool value)
        {
            supportHandConstraint.weight = value ? 1f : 0f;
        }

        public void ApplyAimProfile(
            Vector3 offset,
            Axis aimAxis,
            Axis upAxis)
        {
            var data = aimConstraint.data;
            data.offset = offset;
            data.aimAxis = MapAxis(aimAxis);
            data.upAxis = MapAxis(upAxis);
            aimConstraint.data = data;
        }

        public void ApplySpineProfile(Vector3 offset)
        {
            var data=spineConstraint.data;
            data.offset = offset;
            spineConstraint.data = data;
        }

        private static MultiAimConstraintData.Axis MapAxis(Axis axis) => axis switch
        {
            Axis.X => MultiAimConstraintData.Axis.X,
            Axis.Y => MultiAimConstraintData.Axis.Y,
            Axis.Z => MultiAimConstraintData.Axis.Z,
            _ => MultiAimConstraintData.Axis.X
        };
    }
}