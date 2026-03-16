using _Project.Configs;
using UniRx;
using UnityEngine;

namespace _Project.Animals.Movement
{
	public class LinearMovementStrategy : IMovementStrategy
	{
		private readonly SnakeConfig _config;
		private readonly CompositeDisposable _disposable = new CompositeDisposable();
		
		private Rigidbody _rigidbody;
		private Vector3 _direction;
		private Bounds _bounds;
		
		public LinearMovementStrategy(SnakeConfig config)
		{
			_config = config;
		}
		

		public void Initialize(Rigidbody rigidbody)
		{
			_rigidbody = rigidbody;
			PickRandomDirection();
 
			Observable
				.Interval(System.TimeSpan.FromSeconds(_config.DirectionChangeInterval))
				.Subscribe(_ => PickRandomDirection())
				.AddTo(_disposable);
		}
 
		public void Tick(Rigidbody rigidbody, Bounds bounds)
		{
			_bounds = bounds;
			HandleBounds();
 
			rigidbody.MovePosition(
				rigidbody.position + _direction * (_config.MoveSpeed * Time.fixedDeltaTime)
			);
		}
		
		private void PickRandomDirection()
		{
			var angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
			_direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
		}
		
		private void HandleBounds()
		{
			if (_rigidbody == null) return;
 
			var pos = _rigidbody.position;
 
			if (pos.x <= _bounds.min.x || pos.x >= _bounds.max.x)
				_direction.x = -_direction.x;
 
			if (pos.z <= _bounds.min.z || pos.z >= _bounds.max.z)
				_direction.z = -_direction.z;
 
			pos.x = Mathf.Clamp(pos.x, _bounds.min.x, _bounds.max.x);
			pos.z = Mathf.Clamp(pos.z, _bounds.min.z, _bounds.max.z);
			_rigidbody.position = pos;
		}
 
		public void Dispose()
		{
			_disposable.Dispose();
		}
	}
}