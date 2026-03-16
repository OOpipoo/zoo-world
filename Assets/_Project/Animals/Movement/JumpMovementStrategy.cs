using System;
using _Project.Animals.Base;
using _Project.Configs;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Animals.Movement
{
	public class JumpMovementStrategy : IMovementStrategy
	{
		private readonly FrogConfig _config;
		private readonly CompositeDisposable _disposable = new CompositeDisposable();
		
		private Rigidbody _rigidbody;
		private Vector3 _jumpDirection;
		private Bounds _bounds;
		
		public JumpMovementStrategy(FrogConfig config)
		{
			_config = config;
		}

		public void Initialize(Rigidbody rigidbody)
		{
			_rigidbody = rigidbody;
			PickRandomDirection();
			
			Observable
				.Interval(TimeSpan.FromSeconds(_config.JumpInterval))
				.Subscribe(_ => Jump())
				.AddTo(_disposable);
		}
		
		public void Tick(Rigidbody rigidbody, Bounds bounds)
		{
			_bounds = bounds;
			ClampToBounds();
		}
		
		private void Jump()
		{
			PickRandomDirection();
			_rigidbody.AddForce(_jumpDirection * _config.JumpForce, ForceMode.Impulse);
		}
		
		private void PickRandomDirection()
		{
			var angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
			_jumpDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
		}
		
		private void ClampToBounds()
		{
			if (_rigidbody == null) 
				return;
 
			var pos = _rigidbody.position;
			var needsRedirect = false;
 
			if (pos.x <= _bounds.min.x || pos.x >= _bounds.max.x)
			{
				_jumpDirection.x = -_jumpDirection.x;
				needsRedirect = true;
			}
 
			if (pos.z <= _bounds.min.z || pos.z >= _bounds.max.z)
			{
				_jumpDirection.z = -_jumpDirection.z;
				needsRedirect = true;
			}
 
			if (needsRedirect)
			{
				pos.x = Mathf.Clamp(pos.x, _bounds.min.x, _bounds.max.x);
				pos.z = Mathf.Clamp(pos.z, _bounds.min.z, _bounds.max.z);
				_rigidbody.position = pos;
			}
		}
		
		public void Dispose()
		{
			_disposable.Dispose();
		}
	}
}