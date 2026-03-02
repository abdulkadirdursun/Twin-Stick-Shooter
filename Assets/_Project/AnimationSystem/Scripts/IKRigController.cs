using TwinStickShooter.AnimationSystem.Enums;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TwinStickShooter.AnimationSystem
{
    public class IKRigController : MonoBehaviour
    {
        [SerializeField] private AnimationController animationController;
        [Header("Rigs")]
        [SerializeField] private Rig baseballBatAimRig;
        [SerializeField] private Rig pistolAimRig;

        private Rig _activeRig;

        public void SetActive(bool value)
        {
            if (_activeRig) _activeRig.weight = 0f;
            if (!value || !TryGetRig(out var rig))
                return;

            _activeRig = rig;
            _activeRig.weight = 1f;
        }

        private bool TryGetRig(out Rig rig)
        {
            var animatorLayer = animationController.GetActiveLayer();
            switch (animatorLayer)
            {
                case AnimatorLayer.UpperBody_BaseballBat:
                    rig = baseballBatAimRig;
                    return true;
                case AnimatorLayer.UpperBody_Pistol:
                    rig = pistolAimRig;
                    return true;
                default:
                    rig = null;
                    return false;
            }
        }
    }
}