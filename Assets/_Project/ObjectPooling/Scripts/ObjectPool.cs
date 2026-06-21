using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TwinStickShooter.ObjectPooling
{
    public class ObjectPool<T> where T : Object
    {
        #region Constructor

        public ObjectPool(
            T prefab,
            Transform parent,
            int startPoolSize,
            int maxPoolSize,
            Action<T, ObjectPool<T>> onCreate = null,
            Action<T> onGet = null,
            Action<T> onRelease = null)
        {
            _prefab = prefab;
            _parent = parent;
            _maxPoolSize = maxPoolSize;
            _onCreate = onCreate;
            _onGet = onGet;
            _onRelease = onRelease;

            for (int i = 0; i < startPoolSize; i++)
            {
                _passiveObjects.Enqueue(CreateObject());
            }
        }

        #endregion

        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly int _maxPoolSize;
        private readonly Action<T, ObjectPool<T>> _onCreate;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;

        private readonly HashSet<T> _activeObjects = new();
        private readonly Queue<T> _passiveObjects = new();

        private int _currentPoolSize;

        public T Get()
        {
            var obj = _passiveObjects.Count > 0 ? _passiveObjects.Dequeue() : CreateObject();
            _activeObjects.Add(obj);
            _onGet?.Invoke(obj);
            return obj;
        }

        public void ReleaseRequest(T obj)
        {
            if (!_activeObjects.Remove(obj)) return;
            ReleaseObject(obj);
        }

        public void ReleaseAll()
        {
            if (_activeObjects.Count == 0) return;
            _activeObjects.RemoveWhere(
                obj =>
                {
                    ReleaseObject(obj);
                    return true;
                });
            _activeObjects.Clear();
        }

        private void ReleaseObject(T obj)
        {
            _onRelease?.Invoke(obj);
            if (_currentPoolSize > _maxPoolSize)
            {
                DestroyObject(obj);
                return;
            }

            _passiveObjects.Enqueue(obj);
        }

        private T CreateObject()
        {
            var newObj = Object.Instantiate(_prefab, _parent, false);
            _onCreate?.Invoke(newObj, this);
            _currentPoolSize++;
            return newObj;
        }

        private void DestroyObject(T obj)
        {
            Object.Destroy(obj);
            _currentPoolSize--;
        }
    }
}