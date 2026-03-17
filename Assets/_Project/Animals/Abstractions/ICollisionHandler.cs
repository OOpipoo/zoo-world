namespace _Project.Animals.Abstractions
{
	public interface ICollisionHandler
	{
		void HandleCollision(IAnimal self, IAnimal other);
	}
}
