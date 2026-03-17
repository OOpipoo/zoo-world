using UniRx;

namespace _Project.Infrastructures.Services
{
	public class ScoreService
	{
		private readonly ReactiveProperty<int> _preyDeathCount = new ReactiveProperty<int>(0);
		private readonly ReactiveProperty<int> _predatorDeathCount = new ReactiveProperty<int>(0);

		public IReadOnlyReactiveProperty<int> PreyDeathCount => _preyDeathCount;
		public IReadOnlyReactiveProperty<int> PredatorDeathCount => _predatorDeathCount;

		public void AddPreyDeath() => _preyDeathCount.Value++;
		public void AddPredatorDeath() => _predatorDeathCount.Value++;
	}
}
