using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AKD.Toolkit.ObjectPooling
{
    public class ObjectPool<T> where T : Object
    {
        #region Constructor

        public ObjectPool(T prefab,
            int startPoolSize = 0,
            int maxPoolSize = 0,
            Transform parent = null,
            Action<T, ObjectPool<T>> createAction = null,
            Action<T> getAction = null,
            Action<T> releaseAction = null)
        {
            _objectPrefab = prefab;
            _maxPoolSize = Mathf.Clamp(maxPoolSize, 1, int.MaxValue);
            startPoolSize = Mathf.Clamp(startPoolSize, 0, _maxPoolSize);
            _currentPoolSize = 0;
            _parent = parent;
            _passiveObjects = new(startPoolSize);
            _activeObjects = new();
            _createAction = createAction;
            _getAction = getAction;
            _releaseAction = releaseAction;

            for (int i = 0; i < startPoolSize; i++)
            {
                _passiveObjects.Enqueue(CreateObject());
            }
        }

        #endregion

        private readonly T _objectPrefab;
        private readonly Transform _parent;
        private readonly Queue<T> _passiveObjects;
        private readonly HashSet<T> _activeObjects;
        private readonly Action<T, ObjectPool<T>> _createAction;
        private readonly Action<T> _getAction;
        private readonly Action<T> _releaseAction;
        private readonly int _maxPoolSize;
        private int _currentPoolSize;

        private int PassiveObjectsCount => _passiveObjects.Count;
        private int ActiveObjectsCount => _activeObjects.Count;

        public T Get()
        {
            T obj = PassiveObjectsCount == 0 ? CreateObject() : _passiveObjects.Dequeue();
            _activeObjects.Add(obj);
            _getAction?.Invoke(obj);
            return obj;
        }

        public void Release(T obj)
        {
            if (!_activeObjects.Remove(obj)) return;
            _releaseAction?.Invoke(obj);
            if (_currentPoolSize > _maxPoolSize)
            {
                Object.Destroy(obj);
                _currentPoolSize--;
                return;
            }

            _passiveObjects.Enqueue(obj);
        }

        public void Release(List<T> objects)
        {
            foreach (var obj in objects) Release(obj);
        }

        public void ReleaseAll()
        {
            if (ActiveObjectsCount == 0) return;
            var objectToRelease = _activeObjects.ToArray();

            foreach (var obj in objectToRelease)
            {
                Release(obj);
            }
        }

        private T CreateObject()
        {
            T newObject = Object.Instantiate(_objectPrefab, _parent, false);
            _createAction?.Invoke(newObject, this);
            _currentPoolSize++;
            return newObject;
        }
    }
}