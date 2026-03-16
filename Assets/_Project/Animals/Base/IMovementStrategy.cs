using System;
using UnityEngine;

namespace _Project.Animals.Base
{
	public interface IMovementStrategy : IDisposable
	{
		/// <param name="rigidbody">Rigidbody animal for physics application</param>
		/// <param name="bounds">Playing field boundaries for reversal</param>
		void Tick(Rigidbody rigidbody, Bounds bounds);
		
		void Initialize(Rigidbody rigidbody);
		
		void OnCollision(Collision collision);
	}
}