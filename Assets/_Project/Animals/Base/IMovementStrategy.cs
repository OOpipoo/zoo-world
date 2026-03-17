using System;
using UnityEngine;

namespace _Project.Animals.Base
{
	public interface IMovementStrategy : IDisposable
	{
		void Tick(Rigidbody rigidbody, Bounds bounds);
		void Initialize(Rigidbody rigidbody);
		void OnCollision(Collision collision);
	}
}
