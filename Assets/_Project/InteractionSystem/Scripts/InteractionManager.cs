using System.Collections.Generic;
using TwinStickShooter.InteractionSystem.Interfaces;
using UnityEngine;

namespace TwinStickShooter.InteractionSystem
{
    public class InteractionManager : MonoBehaviour
    {
        private readonly List<IInteractable> _possibleInteractables = new();

        public void AddInteractable(IInteractable interactable)
        {
            if (_possibleInteractables.Contains(interactable)) return;
            _possibleInteractables.Add(interactable);
        }

        public void RemoveInteractable(IInteractable interactable)
        {
            _possibleInteractables.Remove(interactable);
        }

        private void Interact()
        {
            if (_possibleInteractables.Count == 0) return;
            SortThePossibleInteractables();
            var interactable = _possibleInteractables[0];
            interactable.Interact();
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
    }
}