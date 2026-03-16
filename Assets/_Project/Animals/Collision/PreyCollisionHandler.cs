using _Project.Animals.Base;

namespace _Project.Animals.Collision
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