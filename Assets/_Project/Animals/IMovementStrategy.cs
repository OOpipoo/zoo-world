using UnityEngine;

namespace _Project.Animals
{
	public interface IMovementStrategy
	{
		/// <param name="rigidbody">Rigidbody animal for physics application</param>
		/// <param name="bounds">Playing field boundaries for reversal</param>
		void Tick(Rigidbody rigidbody, Bounds bounds);
		
		void Initialize(Rigidbody rigidbody);
	}
}