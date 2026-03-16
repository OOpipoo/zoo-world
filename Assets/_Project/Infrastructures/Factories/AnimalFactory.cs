using System;
using System.Collections.Generic;
using _Project.Animals;
using _Project.Animals.Base;
using _Project.Animals.Collision;
using _Project.Animals.Movement;
using _Project.Configs;
using _Project.Infrastructures.Services;
using _Project.Ui;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace _Project.Infrastructures.Factories
{
	public class AnimalFactory
    {
        private readonly DiContainer _container;
        private readonly GameBoundsService _boundsService;
        private readonly AnimalRegistry _registry;
        private readonly ScoreService _scoreService;
        private readonly TastyLabel _tastyLabelPrefab;
        private readonly SpawnLimitsConfig _spawnLimits;
        private readonly Dictionary<AnimalTypeSO, AnimalConfig> _configs;
 
        public AnimalFactory(
            DiContainer container,
            GameBoundsService boundsService,
            AnimalRegistry registry,
            ScoreService scoreService,
            TastyLabel tastyLabelPrefab,
            SpawnLimitsConfig spawnLimits,
            List<AnimalConfig> configs)
        {
            _container = container;
            _boundsService = boundsService;
            _registry = registry;
            _scoreService = scoreService;
            _tastyLabelPrefab = tastyLabelPrefab;
            _spawnLimits = spawnLimits;
 
            _configs = new Dictionary<AnimalTypeSO, AnimalConfig>();
            foreach (var config in configs)
                _configs[config.AnimalType] = config;
        }
 
        public AnimalPresenter Create(AnimalTypeSO animalType)
        {
            if (!_configs.TryGetValue(animalType, out var config))
            {
                Debug.LogError($"[AnimalFactory] No config found for type: {animalType.DisplayName}");
                return null;
            }
 
            var spawnPosition = GetRandomSpawnPosition(config);
            var view = SpawnView(config.Prefab, spawnPosition);
            var model = new AnimalModel(config);
            var movementStrategy = CreateMovementStrategy(config);
            var collisionHandler = CreateCollisionHandler(config);
 
            TastyLabel tastyLabel = null;
            if (!config.IsPrey)
                tastyLabel = Object.Instantiate(_tastyLabelPrefab, view.transform);
 
            var presenter = new AnimalPresenter(
                model,
                view,
                movementStrategy,
                collisionHandler,
                _boundsService,
                _registry,
                _scoreService,
                tastyLabel
            );
 
            presenter.Initialize();
            return presenter;
        }
 
        private AnimalView SpawnView(GameObject prefab, Vector3 position)
        {
            var instance = _container.InstantiatePrefab(prefab, position, Quaternion.identity, null);
            return instance.GetComponent<AnimalView>();
        }
 
        private IMovementStrategy CreateMovementStrategy(AnimalConfig config)
        {
            return config switch
            {
                FrogConfig frogConfig   => new JumpMovementStrategy(frogConfig),
                SnakeConfig snakeConfig => new LinearMovementStrategy(snakeConfig),
                _ => throw new System.ArgumentException($"Unknown config type: {config.GetType().Name}")
            };
        }
 
        private ICollisionHandler CreateCollisionHandler(AnimalConfig config)
        {
            return config.IsPrey
                ? new PreyCollisionHandler()
                : (ICollisionHandler) new PredatorCollisionHandler();
        }
 
        private static readonly Collider[] _overlapBuffer = new Collider[4];
        private static readonly int AnimalsLayerMask = 1 << LayerMask.NameToLayer("Animals");
 
        private Vector3 GetRandomSpawnPosition(AnimalConfig config)
        {
            var bounds = _boundsService.Bounds;
            const int maxAttempts = 20;
 
            for (int i = 0; i < maxAttempts; i++)
            {
                var candidate = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    config.SpawnHeight,
                    Random.Range(bounds.min.z, bounds.max.z)
                );
 
                var hits = Physics.OverlapSphereNonAlloc(
                    candidate,
                    _spawnLimits.MinSpawnDistance,
                    _overlapBuffer,
                    AnimalsLayerMask
                );
 
                if (hits == 0)
                    return candidate;
            }
 
            Debug.LogWarning("[AnimalFactory] Could not find free spawn position after 20 attempts.");
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                config.SpawnHeight,
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }
    }
}
