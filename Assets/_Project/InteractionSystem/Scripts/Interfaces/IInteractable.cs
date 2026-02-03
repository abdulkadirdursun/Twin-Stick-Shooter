using UnityEngine;

namespace TwinStickShooter.InteractionSystem
{
    public interface IInteractable
    {
        public Vector3 Position { get; }
        public void Interact();
    }
}