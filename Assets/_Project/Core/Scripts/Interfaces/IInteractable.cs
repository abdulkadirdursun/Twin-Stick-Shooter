using UnityEngine;

namespace TwinStickShooter.Core
{
    public interface IInteractable
    {
        public Vector3 Position { get; }
        public void Interact();
        public void ShowInteractableIndicator();
        public void HideInteractableIndicator();
    }
}