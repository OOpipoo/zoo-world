using _Project.Configs;
using UniRx;

namespace _Project.Animals.Base
{
	public class AnimalModel : IAnimal
	{
		private readonly ReactiveProperty<bool> _isAlive = new ReactiveProperty<bool>(true);
 
		public AnimalTypeSO AnimalType { get; }
		public bool IsPrey { get; }
		public bool IsAlive => _isAlive.Value;
		public IReadOnlyReactiveProperty<bool> IsAliveProperty => _isAlive;
		public AnimalConfig Config { get; }
 
		
		public AnimalModel(AnimalConfig config)
		{
			Config = config;
			AnimalType = config.AnimalType;
			IsPrey = config.IsPrey;
		}
 
		public void Die()
		{
			if (!_isAlive.Value) return;
			_isAlive.Value = false;
		}
	}
}