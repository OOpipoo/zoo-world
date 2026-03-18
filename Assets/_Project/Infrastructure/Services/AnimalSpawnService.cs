using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
using _Project.Configs;
using _Project.Infrastructure.Factories;
using _Project.Infrastructure.Services;
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
			Observable
				.Timer(TimeSpan.FromSeconds(Random.Range(MinSpawnInterval, MaxSpawnInterval)))
				.Subscribe(_ =>
				{
					TrySpawnRandom();
					ScheduleNextSpawn();
				})
				.AddTo(_disposable);
		}

		private void TrySpawnRandom()
		{
			var all = _registry.GetAll().ToList();
			var preyCount = all.Count(p => p.Animal.IsPrey);
			var predatorCount = all.Count - preyCount;

			var available = _configs.Where(c =>
				c.IsPrey ? preyCount < _spawnLimits.MaxPrey : predatorCount < _spawnLimits.MaxPredators
			).ToList();

			if (available.Count == 0)
			{
				Debug.Log("[AnimalSpawnService] All limits reached, skipping spawn.");
				return;
			}

			_factory.Create(available[Random.Range(0, available.Count)].AnimalType);
		}

		public void Restart()
		{
			_disposable.Clear();
			ScheduleNextSpawn();
		}

		public void Dispose() => _disposable.Dispose();
	}
}
