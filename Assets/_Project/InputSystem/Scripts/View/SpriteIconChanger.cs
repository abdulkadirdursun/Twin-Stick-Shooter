using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwinStickShooter.InputSystem.View
{
    public class SpriteIconChanger : MonoBehaviour
    {
        #region Struct

        [Serializable]
        public struct IconInfo
        {
            public ControlSchemeType controlScheme;
            public GameObject iconObject;
        }

        #endregion

        [SerializeField] private InputControlSchemeData inputControlScheme;
        [SerializeField] private IconInfo[] iconInfos;
        [SerializeField] private GameObject contentParent;

        private readonly Dictionary<ControlSchemeType, GameObject> _iconLookup = new();
        private GameObject _activeInputIcon;
        private bool _isActive;

        public void Show()
        {
            _isActive = true;
            inputControlScheme.ControlSchemeTypeChanged += SetControlSchemeIcon;
            SetControlSchemeIcon(inputControlScheme.CurrentControlType);
            contentParent.SetActive(true);
        }

        public void Hide()
        {
            _isActive = false;
            inputControlScheme.ControlSchemeTypeChanged -= SetControlSchemeIcon;
            contentParent.SetActive(false);
        }

        private void SetControlSchemeIcon(ControlSchemeType controlSchemeType)
        {
            _activeInputIcon?.SetActive(false);
            if (!_iconLookup.TryGetValue(controlSchemeType, out _activeInputIcon)) return;
            _activeInputIcon.SetActive(true);
        }

        #region MonoBehaviour Methods

        private void Awake()
        {
            foreach (var iconInfo in iconInfos)
            {
                _iconLookup.Add(iconInfo.controlScheme, iconInfo.iconObject);
                iconInfo.iconObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (_isActive)
                Hide();
        }

        #endregion
    }
}