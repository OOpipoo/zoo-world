namespace _Project.Animals.Base
{
	public interface ICollisionHandler
	{
		void HandleCollision(IAnimal self, IAnimal other);
	}
}
