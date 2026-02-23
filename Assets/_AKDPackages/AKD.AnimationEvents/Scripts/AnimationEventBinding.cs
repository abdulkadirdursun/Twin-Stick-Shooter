using System;
using UnityEngine.Events;

namespace AKD.AnimationEvents
{
    [Serializable]
    public struct AnimationEventBinding
    {
        public int layerIndex;
        public string eventName;
        public UnityEvent response;
    }
}
