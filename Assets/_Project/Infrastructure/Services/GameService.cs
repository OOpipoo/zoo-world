using System.Linq;
using _Project.Infrastructure.Pools;
using _Project.Infrastructures.Services;

namespace _Project.Infrastructure.Services
{
	public class GameService
	{
		private readonly AnimalRegistry _registry;
		private readonly AnimalPool _animalPool;
		private readonly ScoreService _scoreService;
		private readonly AnimalSpawnService _spawnService;

		public GameService(
			AnimalRegistry registry,
			AnimalPool animalPool,
			ScoreService scoreService,
			AnimalSpawnService spawnService)
		{
			_registry = registry;
			_animalPool = animalPool;
			_scoreService = scoreService;
			_spawnService = spawnService;
		}

		public void Restart()
		{
			var presenters = _registry.GetAll().ToList();
			foreach (var presenter in presenters)
			{
				_registry.Unregister(presenter.View);
				_animalPool.Return(presenter.View, presenter.Animal.AnimalType);
				presenter.Dispose();
			}

			_scoreService.Reset();
			_spawnService.Restart();
		}
	}
}
