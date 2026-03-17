using _Project.Configs;
using UniRx;

namespace _Project.Animals.Abstractions
{
	public interface IAnimal
	{
		AnimalTypeSO AnimalType { get; }
		bool IsPrey { get; }
		bool IsAlive { get; }
		IReadOnlyReactiveProperty<bool> IsAliveProperty { get; }
		void Die();
	}
}
