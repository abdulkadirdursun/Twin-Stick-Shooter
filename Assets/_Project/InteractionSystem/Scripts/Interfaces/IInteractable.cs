using UnityEngine;

namespace TwinStickShooter.InteractionSystem.Interfaces
{
    public interface IInteractable
    {
        public Vector3 Position { get; }
        public void Interact();
    }
}