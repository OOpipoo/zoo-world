using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace _Project.Infrastructure.Pools
{
    public abstract class BasePool : MonoBehaviour
    {
        private readonly Dictionary<Type, Stack<MonoBehaviour>> _pool = new();
        private readonly Dictionary<Type, MonoBehaviour> _prefabs = new();

        protected virtual void Awake() { }

        protected void Fill<T>(T prefab, int initialSize) where T : MonoBehaviour
        {
            var type = typeof(T);
            _prefabs.TryAdd(type, prefab);

            if (!_pool.ContainsKey(type))
                _pool[type] = new Stack<MonoBehaviour>();

            for (var i = 0; i < initialSize; i++)
            {
                var obj = CreateObject<T>();
                obj.gameObject.SetActive(false);
                _pool[type].Push(obj);
            }
        }

        protected T Get<T>() where T : MonoBehaviour
        {
            var type = typeof(T);

            if (_pool.TryGetValue(type, out var stack) && stack.Count > 0)
            {
                var obj = stack.Pop() as T;
                if (obj != null && obj.gameObject != null)
                    return obj;
            }

            return CreateObject<T>();
        }

        protected void Return<T>(T obj) where T : MonoBehaviour
        {
            if (obj == null || obj.gameObject == null) return;

            obj.gameObject.SetActive(false);
            obj.transform.SetParent(transform);

            var type = obj.GetType();
            if (!_pool.ContainsKey(type))
                _pool[type] = new Stack<MonoBehaviour>();

            _pool[type].Push(obj);
        }

        protected void Clear()
        {
            foreach (var stack in _pool.Values)
                foreach (var obj in stack)
                    if (obj != null && obj.gameObject != null)
                        Object.Destroy(obj.gameObject);
            _pool.Clear();
        }

        private T CreateObject<T>() where T : MonoBehaviour
        {
            var type = typeof(T);
            if (!_prefabs.TryGetValue(type, out var prefab))
            {
                Debug.LogError($"[BasePool] No prefab for: {type.Name}");
                return null;
            }
            var instance = Object.Instantiate(prefab.gameObject, transform);
            return instance.GetComponent<T>();
        }
    }
}