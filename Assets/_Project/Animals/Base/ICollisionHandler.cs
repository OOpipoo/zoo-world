namespace _Project.Animals.Base
{	
	public interface ICollisionHandler
	{
		/// <param name="self">The animal involved in the collision</param>
		/// <param name="other">The animal they encountered</param>
		void HandleCollision(IAnimal self, IAnimal other);
	}
}