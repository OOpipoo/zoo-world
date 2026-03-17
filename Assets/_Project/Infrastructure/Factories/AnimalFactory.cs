using System.Collections.Generic;
using _Project.Animals.Abstractions;
using _Project.Animals.CollisionHandlers;
using _Project.Animals.Core;
using _Project.Animals.Movement;
using _Project.Configs;
using _Project.Infrastructure.Pools;
using _Project.Infrastructure.Services;
using _Project.Infrastructures.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Infrastructure.Factories
{
	public class AnimalFactory
    {
        private readonly AnimalPool _animalPool;
        private readonly GameBoundsService _boundsService;
        private readonly AnimalRegistry _registry;
        private readonly ScoreService _scoreService;
        private readonly TastyLabelPool _tastyLabelPool;
        private readonly SpawnLimitsConfig _spawnLimits;
        private readonly Dictionary<AnimalTypeSO, AnimalConfig> _configs;

        private static readonly Collider[] _overlapBuffer = new Collider[4];
        private static readonly int AnimalsLayerMask = 1 << LayerMask.NameToLayer("Animals");

        public AnimalFactory(
            AnimalPool animalPool,
            GameBoundsService boundsService,
            AnimalRegistry registry,
            ScoreService scoreService,
            TastyLabelPool tastyLabelPool,
            SpawnLimitsConfig spawnLimits,
            List<AnimalConfig> configs)
        {
            _animalPool = animalPool;
            _boundsService = boundsService;
            _registry = registry;
            _scoreService = scoreService;
            _tastyLabelPool = tastyLabelPool;
            _spawnLimits = spawnLimits;

            _configs = new Dictionary<AnimalTypeSO, AnimalConfig>();
            foreach (var config in configs)
                _configs[config.AnimalType] = config;
        }

        public AnimalPresenter Create(AnimalTypeSO animalType)
        {
            if (!_configs.TryGetValue(animalType, out var config))
            {
                Debug.LogError($"[AnimalFactory] No config for: {animalType.DisplayName}");
                return null;
            }

            var view = _animalPool.Get(animalType, GetRandomSpawnPosition(config));

            if (view == null)
            {
                Debug.LogError($"[AnimalFactory] AnimalPool returned null view for {animalType.DisplayName}");
                return null;
            }

            var presenter = new AnimalPresenter(
                new AnimalModel(config),
                view,
                CreateMovementStrategy(config),
                CreateCollisionHandler(config),
                _boundsService,
                _registry,
                _scoreService,
                _animalPool,
                config.IsPrey ? null : _tastyLabelPool
            );

            presenter.Initialize();
            return presenter;
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
            return config.IsPrey ? null : new PredatorCollisionHandler();
        }

        private Vector3 GetRandomSpawnPosition(AnimalConfig config)
        {
            var bounds = _boundsService.Bounds;
            var radius = config.BoundsRadius;
            const int maxAttempts = 20;

            var minX = bounds.min.x + radius;
            var maxX = bounds.max.x - radius;
            var minZ = bounds.min.z + radius;
            var maxZ = bounds.max.z - radius;

            for (int i = 0; i < maxAttempts; i++)
            {
                var candidate = new Vector3(
                    Random.Range(minX, maxX),
                    config.SpawnHeight,
                    Random.Range(minZ, maxZ)
                );

                if (Physics.OverlapSphereNonAlloc(candidate, _spawnLimits.MinSpawnDistance, _overlapBuffer, AnimalsLayerMask) == 0)
                    return candidate;
            }

            Debug.LogWarning("[AnimalFactory] Could not find free spawn position after 20 attempts.");
            return new Vector3(Random.Range(minX, maxX), config.SpawnHeight, Random.Range(minZ, maxZ));
        }
    }
}
