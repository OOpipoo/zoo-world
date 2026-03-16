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
        private readonly Dictionary<AnimalTypeSO, AnimalConfig> _configs;
 
        public AnimalFactory(
            DiContainer container,
            GameBoundsService boundsService,
            AnimalRegistry registry,
            ScoreService scoreService,
            TastyLabel tastyLabelPrefab,
            List<AnimalConfig> configs)
        {
            _container = container;
            _boundsService = boundsService;
            _registry = registry;
            _scoreService = scoreService;
            _tastyLabelPrefab = tastyLabelPrefab;
 
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
 
            var spawnPosition = GetRandomSpawnPosition();
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
                _ => throw new ArgumentException($"Unknown config type: {config.GetType().Name}")
            };
        }
 
        private ICollisionHandler CreateCollisionHandler(AnimalConfig config)
        {
            return config.IsPrey
                ? new PreyCollisionHandler()
                : (ICollisionHandler) new PredatorCollisionHandler();
        }
 
        private Vector3 GetRandomSpawnPosition()
        {
            var bounds = _boundsService.Bounds;
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                0f,
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }
    }
}