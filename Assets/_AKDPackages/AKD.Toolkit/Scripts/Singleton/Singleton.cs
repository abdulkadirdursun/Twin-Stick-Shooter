using UnityEngine;

namespace AKD.Toolkit.Singleton
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }
        public static bool IsInstanceExist => Instance != null;

        protected virtual void OnAwake()
        {
        }

        #region MonoBehaviour Methods

        private void Awake()
        {
            if (IsInstanceExist)
            {
                Debug.LogError($"Instance of {typeof(T)} already exists.");
                Destroy(this);
                return;
            }

            Instance = (T)this;
            OnAwake();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        #endregion
    }
}