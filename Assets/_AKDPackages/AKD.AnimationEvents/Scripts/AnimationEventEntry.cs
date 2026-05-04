using System;
using UnityEngine;

namespace AKD.AnimationEvents
{
    [Serializable]
    public struct AnimationEventEntry
    {
        public string eventName;
        public AnimationEventType eventType;

        [Range(0f, 1f)]
        public float normalizedTime;
    }
}
