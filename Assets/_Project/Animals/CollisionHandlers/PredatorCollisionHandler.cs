using _Project.Animals.Abstractions;

namespace _Project.Animals.CollisionHandlers
{
	public class PredatorCollisionHandler : ICollisionHandler
	{
		public void HandleCollision(IAnimal self, IAnimal other)
		{
			if (!other.IsAlive) return;
			if (!self.IsAlive) return;

			if (other.IsPrey)
			{
				other.Die();
				return;
			}

			var selfHash = self.GetHashCode();
			var otherHash = other.GetHashCode();

			if (selfHash == otherHash)
			{
				self.Die();
				return;
			}

			if (selfHash < otherHash)
				self.Die();
			else
				other.Die();
		}
	}
}