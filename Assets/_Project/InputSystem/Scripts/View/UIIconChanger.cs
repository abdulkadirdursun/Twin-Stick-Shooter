using System;
using System.Collections.Generic;
using AKD.Toolkit.Extensions;
using UnityEngine;

namespace TwinStickShooter.InputSystem.View
{
    public class UIIconChanger : MonoBehaviour
    {
        #region Struct

        [Serializable]
        public struct IconInfo
        {
            public ControlSchemeType controlScheme;
            public CanvasGroup iconCanvasGroup;
        }

        #endregion

        [SerializeField] private IconInfo[] iconInfos;

        private readonly Dictionary<ControlSchemeType, CanvasGroup> _iconLookup = new();
        private CanvasGroup _activeCanvasGroup;

        private void SetControlSchemeIcon(ControlSchemeType controlSchemeType)
        {
            _activeCanvasGroup?.SetActive(false, false);
            if (!_iconLookup.TryGetValue(controlSchemeType, out _activeCanvasGroup)) return;
            _activeCanvasGroup.SetActive(true, false);
        }

        #region MonoBehaviour Methods

        private void Awake()
        {
            foreach (var iconInfo in iconInfos)
            {
                _iconLookup.Add(iconInfo.controlScheme, iconInfo.iconCanvasGroup);
            }
        }

        private void OnEnable()
        {
            InputControlScheme.OnControlSchemeTypeChanged += SetControlSchemeIcon;
        }

        private void Start()
        {
            SetControlSchemeIcon(InputControlScheme.CurrentControlType);
        }

        private void OnDisable()
        {
            InputControlScheme.OnControlSchemeTypeChanged -= SetControlSchemeIcon;
        }

        #endregion
    }
}