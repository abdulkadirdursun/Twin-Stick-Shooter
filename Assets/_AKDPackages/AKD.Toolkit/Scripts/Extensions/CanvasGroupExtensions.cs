using UnityEngine;

namespace AKD.Toolkit.Extensions
{
    public static class CanvasGroupExtensions
    {
        public static void SetActive(this CanvasGroup canvasGroup, bool value, bool isInteractable = true)
        {
            if (canvasGroup == null)
            {
                Debug.LogError("Canvas Group you try to change activity is null");
                return;
            }
            canvasGroup.alpha = value ? 1f : 0f;
            if (!isInteractable) return;
            canvasGroup.interactable = value;
            canvasGroup.blocksRaycasts = value;
        }
    }
}