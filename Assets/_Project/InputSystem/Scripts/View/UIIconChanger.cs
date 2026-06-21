using System;
using System.Collections.Generic;
using TwinStickShooter.Core.Utilities;
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

        [SerializeField] private InputControlSchemeData inputControlSchemeData;
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
            inputControlSchemeData.ControlSchemeTypeChanged += SetControlSchemeIcon;
        }

        private void Start()
        {
            SetControlSchemeIcon(inputControlSchemeData.CurrentControlType);
        }

        private void OnDisable()
        {
            inputControlSchemeData.ControlSchemeTypeChanged -= SetControlSchemeIcon;
        }

        #endregion
    }
}