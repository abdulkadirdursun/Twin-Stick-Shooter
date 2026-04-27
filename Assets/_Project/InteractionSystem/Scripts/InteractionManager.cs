using System.Collections;
using System.Collections.Generic;
using TwinStickShooter.Core;
using TwinStickShooter.InputSystem;
using UnityEngine;

namespace TwinStickShooter.InteractionSystem
{
    public class InteractionManager : MonoBehaviour
    {
        [SerializeField] private float checkNearestInteractableInterval = 0.1f;
        private readonly List<IInteractable> _possibleInteractables = new();

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

        private void SortThePossibleInteractables()
        {
            var originPos = transform.position;
            _possibleInteractables.Sort(CompareDistance);

            int CompareDistance(IInteractable a, IInteractable b)
            {
                var distA = (a.Position - originPos).sqrMagnitude;
                var distB = (b.Position - originPos).sqrMagnitude;
                return distA.CompareTo(distB);
            }
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

            SortThePossibleInteractables();
            var tempNearestInteractable = _possibleInteractables[0];
            if (tempNearestInteractable == _nearestInteractable) return _nearestInteractable != null;
            _nearestInteractable?.HideInteractableIndicator();
            _nearestInteractable = tempNearestInteractable;
            _nearestInteractable.ShowInteractableIndicator();
            return true;
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

        private void OnEnable()
        {
            PlayerInputs.Interact += Interact;
        }

        private void Start()
        {
            StartCoroutine(CheckInteractables());
        }

        private void OnDisable()
        {
            PlayerInputs.Interact -= Interact;
        }

        #endregion
    }
}