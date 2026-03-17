using System;
using UnityEngine;

namespace _Project.Animals.Core
{
	[RequireComponent(typeof(Rigidbody))]
	public class AnimalView : MonoBehaviour
	{
		[SerializeField] private Transform _tastyLabelAnchor;

		public Rigidbody Rigidbody { get; private set; }
		public bool IsPrey { get; set; }
		public Transform TastyLabelAnchor => _tastyLabelAnchor != null ? _tastyLabelAnchor : transform;

		public event Action<AnimalView> OnCollisionEntered;
		public event Action<Collision> OnCollisionPhysics;
		public event Action OnFixedUpdated;

		private void Awake()
		{
			Rigidbody = GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			OnFixedUpdated?.Invoke();
		}

		private void OnCollisionEnter(Collision collision)
		{
			OnCollisionPhysics?.Invoke(collision);

			if (collision.gameObject.TryGetComponent<AnimalView>(out var otherView))
				OnCollisionEntered?.Invoke(otherView);
		}

		public void ResetState()
		{
			Rigidbody.linearVelocity = Vector3.zero;
			Rigidbody.angularVelocity = Vector3.zero;
		}

		public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			transform.position = position;
			transform.rotation = rotation;
			Rigidbody.position = position;
		}
	}
}
