using _Project.Animals.Base;
using UnityEngine;

namespace _Project.Animals.Collision
{
	public class PredatorCollisionHandler : ICollisionHandler
	{
		public void HandleCollision(IAnimal self, IAnimal other)
		{
			if (!other.IsAlive) return;
 
			if (other.IsPrey)
			{
				other.Die();
				return;
			}
 
			DieRandom(self, other);
		}

		private void DieRandom(IAnimal self, IAnimal other)
		{
			var selfDies = Random.value > 0.5f;
			if (selfDies)
				self.Die();
			else
				other.Die();
		}
	}
}