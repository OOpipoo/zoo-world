using System;
using System.Collections.Generic;
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
		private readonly List<AnimalConfig> _configs;
		private readonly CompositeDisposable _disposable = new CompositeDisposable();

		private const float MinSpawnInterval = 1f;
		private const float MaxSpawnInterval = 2f;

		public AnimalSpawnService(AnimalFactory factory, List<AnimalConfig> configs)
		{
			_factory = factory;
			_configs = configs;
		}
		
		public void Initialize()
		{
			ScheduleNextSpawn();
		}

		private void ScheduleNextSpawn()
		{
			var interval = Random.Range(MinSpawnInterval, MaxSpawnInterval);
			
			Observable
				.Timer(TimeSpan.FromSeconds(interval))
				.Subscribe(_ =>
					{
						SpawnRandom();
						ScheduleNextSpawn();
					}
				)
				.AddTo(_disposable);
		}

		private void SpawnRandom()
		{
			if (_configs.Count == 0)
			{
				Debug.LogWarning("[AnimalSpawnService] No configs registered.");
				return;
			}
 
			var randomConfig = _configs[Random.Range(0, _configs.Count)];
			_factory.Create(randomConfig.AnimalType);
		}
 
		public void Dispose()
		{
			_disposable.Dispose();
		}
	}
}