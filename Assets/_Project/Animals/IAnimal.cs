using _Project.Configs;
using UniRx;

namespace _Project.Animals
{
	public interface IAnimal
	{
		AnimalTypeSO AnimalType { get; }
		
		bool IsPrey { get;}
		bool IsAlive { get; }
		
		/// <summary>
		/// Reactive event - shoots once when an animal dies.
		/// Used by ScoreService and AnimalRegistry for subscription.
		/// </summary>
		IReadOnlyReactiveProperty<bool> IsAliveProperty { get; }
		
		void Die();
	}
}