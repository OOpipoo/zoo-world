using UniRx;
using Zenject;

namespace _Project.Infrastructures.Services
{
	public class ScoreService : IInitializable
	{
		private readonly ReactiveProperty<int> _preyDeathCount = new ReactiveProperty<int>(0);
		private readonly ReactiveProperty<int> _predatorDeathCount = new ReactiveProperty<int>(0);
 
		public IReadOnlyReactiveProperty<int> PreyDeathCount => _preyDeathCount;
		public IReadOnlyReactiveProperty<int> PredatorDeathCount => _predatorDeathCount;
 
		public void Initialize() { }
 
		public void AddPreyKill() => _preyDeathCount.Value++;
		public void AddPredatorKill() => _predatorDeathCount.Value++;
	}
}