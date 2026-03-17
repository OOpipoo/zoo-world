using System.Collections.Generic;
using _Project.Animals.Base;
using _Project.Animals.CollisionHandlers;
using _Project.Animals.Movement;
using _Project.Configs;
using _Project.Infrastructures.ObjectPools;
using _Project.Infrastructures.Services;
using _Project.Ui;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Infrastructures.Factories
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
 
            var spawnPosition = GetRandomSpawnPosition(config);
            var view = _animalPool.Get(animalType, spawnPosition);
 
            if (view == null)
            {
                Debug.LogError($"[AnimalFactory] AnimalPool returned null view for {animalType.DisplayName}");
                return null;
            }
            var model = new AnimalModel(config);
            var movementStrategy = CreateMovementStrategy(config);
            var collisionHandler = CreateCollisionHandler(config);
 
            var presenter = new AnimalPresenter(
                model,
                view,
                movementStrategy,
                collisionHandler,
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
            return config.IsPrey
                ? new PreyCollisionHandler()
                : (ICollisionHandler) new PredatorCollisionHandler();
        }
 
        private static readonly Collider[] _overlapBuffer = new Collider[4];
        private static readonly int AnimalsLayerMask = 1 << LayerMask.NameToLayer("Animals");
 
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
                Random.Range(minX, maxX),
                config.SpawnHeight,
                Random.Range(minZ, maxZ)
            );
        }
    }
}
