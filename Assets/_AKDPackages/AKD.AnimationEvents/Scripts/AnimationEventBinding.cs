using System;
using UnityEngine;
using UnityEngine.Events;

namespace AKD.AnimationEvents
{
    [Serializable]
    public struct AnimationEventBinding
    {
        public int stateHash;
        public string stateName;
        public AnimationEventType eventType;

        [Range(0f, 1f)]
        public float normalizedTime;

        public UnityEvent response;
    }
}
