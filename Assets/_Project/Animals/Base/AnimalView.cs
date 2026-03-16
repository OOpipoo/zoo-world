using System;
using UnityEngine;

namespace _Project.Animals
{
	[RequireComponent(typeof(Rigidbody))]
	public class AnimalView : MonoBehaviour
	{
		public Rigidbody Rigidbody { get; private set; }
		
		public event Action<AnimalView> OnCollisionEntered;
		public event Action OnFixedUpdated;

		
		private void Awake()
		{
			Rigidbody = GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			OnFixedUpdated?.Invoke();
		}

		private void OnCollisionEnter(UnityEngine.Collision collision)
		{
			if (collision.gameObject.TryGetComponent<AnimalView>(out var otherView))
				OnCollisionEntered?.Invoke(otherView);
		}
		
		public void Destroy()
		{
			Destroy(gameObject);
		}
	}
}