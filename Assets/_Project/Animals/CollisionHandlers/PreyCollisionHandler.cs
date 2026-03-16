using _Project.Animals.Base;

namespace _Project.Animals.CollisionHandlers
{
	public class PreyCollisionHandler : ICollisionHandler
	{
		public void HandleCollision(IAnimal self, IAnimal other)
		{
			if (!other.IsPrey)
				self.Die();
		}
	}
}