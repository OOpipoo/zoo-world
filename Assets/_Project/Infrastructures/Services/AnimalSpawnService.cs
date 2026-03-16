using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Configs;
using _Project.Infrastructures.Factories;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace _Project.Infrastructures.Services
{
	public class AnimalSpawnService : IInitializable, IDisposable
	{
		private readonly AnimalFactory _factory;
		private readonly AnimalRegistry _registry;
		private readonly List<AnimalConfig> _configs;
		private readonly SpawnLimitsConfig _spawnLimits;
		private readonly CompositeDisposable _disposable = new CompositeDisposable();

		private const float MinSpawnInterval = 1f;
		private const float MaxSpawnInterval = 2f;

		public AnimalSpawnService(
			AnimalFactory factory,
			AnimalRegistry registry,
			List<AnimalConfig> configs,
			SpawnLimitsConfig spawnLimits)
		{
			_factory = factory;
			_registry = registry;
			_configs = configs;
			_spawnLimits = spawnLimits;
		}
 
		public void Initialize() => ScheduleNextSpawn();
 
		private void ScheduleNextSpawn()
		{
			var interval = Random.Range(MinSpawnInterval, MaxSpawnInterval);
 
			Observable
				.Timer(TimeSpan.FromSeconds(interval))
				.Subscribe(_ =>
				{
					TrySpawnRandom();
					ScheduleNextSpawn();
				})
				.AddTo(_disposable);
		}
 
		private void TrySpawnRandom()
		{
			var preyCount = _registry.GetAll().Count(p => p.Animal.IsPrey);
			var predatorCount = _registry.GetAll().Count(p => !p.Animal.IsPrey);
 
			var available = _configs.Where(c =>
			{
				if (c.IsPrey && preyCount >= _spawnLimits.MaxPrey) 
					return false;
				if (!c.IsPrey && predatorCount >= _spawnLimits.MaxPredators) 
					return false;
				return true;
			}).ToList();
 
			if (available.Count == 0)
			{
				Debug.Log("[AnimalSpawnService] All limits reached, skipping spawn.");
				return;
			}
 
			var randomConfig = available[UnityEngine.Random.Range(0, available.Count)];
			_factory.Create(randomConfig.AnimalType);
		}
 
		public void Dispose() => _disposable.Dispose();
	}
}