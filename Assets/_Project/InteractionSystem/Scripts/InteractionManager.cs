using System.Collections;
using System.Collections.Generic;
using TwinStickShooter.Core;
using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.InteractionSystem
{
    public class InteractionManager : MonoBehaviour
    {
        [SerializeField] private GameplayInputs gameplayInputs;
        [SerializeField] private float checkNearestInteractableInterval = 0.1f;
        private readonly List<IInteractable> _possibleInteractables = new();
        private Transform _transform;

        private IInteractable _nearestInteractable;

        public void AddInteractable(IInteractable interactable)
        {
            if (_possibleInteractables.Contains(interactable)) return;
            _possibleInteractables.Add(interactable);
        }

        public void RemoveInteractable(IInteractable interactable)
        {
            if (_nearestInteractable == interactable)
            {
                _nearestInteractable.HideInteractableIndicator();
                _nearestInteractable = null;
            }

            _possibleInteractables.Remove(interactable);
        }

        private void Interact()
        {
            if (_nearestInteractable == null && !CheckForNearestInteractable())
                return;
            _nearestInteractable.Interact();
        }

        private bool CheckForNearestInteractable()
        {
            if (_possibleInteractables.Count == 0)
            {
                if (_nearestInteractable == null) return false;
                _nearestInteractable.HideInteractableIndicator();
                _nearestInteractable = null;
                return false;
            }

            var tempNearestInteractable = FindNearest();
            if (tempNearestInteractable == _nearestInteractable) return _nearestInteractable != null;
            _nearestInteractable?.HideInteractableIndicator();
            _nearestInteractable = tempNearestInteractable;
            _nearestInteractable.ShowInteractableIndicator();
            return true;
        }

        private IInteractable FindNearest()
        {
            var originPos = _transform.position;
            IInteractable nearest = null;
            var nearestSqr = float.MaxValue;

            foreach (var interactable in _possibleInteractables)
            {
                var distanceSqr = (interactable.Position - originPos).sqrMagnitude;
                if (distanceSqr > nearestSqr) continue;
                nearestSqr = distanceSqr;
                nearest = interactable;
            }

            return nearest;
        }

        private IEnumerator CheckInteractables()
        {
            var waitWhileNoInteractable = new WaitWhile(() => _possibleInteractables.Count == 0);
            var waitCheckInterval = new WaitForSecondsRealtime(checkNearestInteractableInterval);
            while (true)
            {
                yield return waitWhileNoInteractable;
                CheckForNearestInteractable();
                yield return waitCheckInterval;
            }
        }

        #region MonoBehaviour Methods

        private void Awake()
        {
            _transform = transform;
        }

        private void OnEnable()
        {
            gameplayInputs.Interact += Interact;
        }

        private void Start()
        {
            StartCoroutine(CheckInteractables());
        }

        private void OnDisable()
        {
            gameplayInputs.Interact -= Interact;
        }

        #endregion
    }
}