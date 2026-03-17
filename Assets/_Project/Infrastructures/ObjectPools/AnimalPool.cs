using UnityEngine;
using _Project.Configs;
using _Project.Animals.Base;
using System.Collections.Generic;

namespace _Project.Infrastructures.ObjectPools
{
	 public class AnimalPool
    {
        private readonly Dictionary<AnimalTypeSO, Stack<AnimalView>> _pools = new();
        private readonly Dictionary<AnimalTypeSO, AnimalView> _prefabs = new();
        private readonly Transform _poolRoot;
 
        public AnimalPool(List<AnimalConfig> configs)
        {
            var root = new GameObject("[Animal Pool]");
            _poolRoot = root.transform;
 
            foreach (var config in configs)
                RegisterConfig(config);
        }
 
        private void RegisterConfig(AnimalConfig config)
        {
            var view = config.Prefab.GetComponent<AnimalView>();
            if (view == null)
            {
                Debug.LogError($"[AnimalPool] {config.AnimalType.DisplayName} prefab has no AnimalView");
                return;
            }
 
            _prefabs[config.AnimalType] = view;
            _pools[config.AnimalType] = new Stack<AnimalView>();
 
            for (int i = 0; i < config.PoolSize; i++)
            {
                var instance = CreateInstance(config.AnimalType);
                instance.gameObject.SetActive(false);
                instance.transform.SetParent(_poolRoot);
                _pools[config.AnimalType].Push(instance);
            }
        }
 
        public AnimalView Get(AnimalTypeSO animalType, Vector3 position)
        {
            AnimalView view;
 
            if (_pools.TryGetValue(animalType, out var stack) && stack.Count > 0)
            {
                view = stack.Pop();
 
                if (view == null || view.gameObject == null)
                    view = CreateInstance(animalType);
            }
            else
            {
                view = CreateInstance(animalType);
            }
 
            view.SetPositionAndRotation(position, Quaternion.identity);
            view.gameObject.SetActive(true);
            return view;
        }
 
        public void Return(AnimalView view, AnimalTypeSO animalType)
        {
            if (view == null || view.gameObject == null) return;
 
            view.ResetState();
            view.gameObject.SetActive(false);
            view.transform.SetParent(_poolRoot);
 
            if (_pools.TryGetValue(animalType, out var stack))
                stack.Push(view);
        }
 
        private AnimalView CreateInstance(AnimalTypeSO animalType)
        {
            if (!_prefabs.TryGetValue(animalType, out var prefab))
            {
                Debug.LogError($"[AnimalPool] No prefab for {animalType.DisplayName}");
                return null;
            }
 
            var go = Object.Instantiate(prefab.gameObject, _poolRoot);
            return go.GetComponent<AnimalView>();
        }
    }
}
