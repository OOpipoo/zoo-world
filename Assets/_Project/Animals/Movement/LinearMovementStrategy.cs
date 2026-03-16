using System;
using _Project.Animals.Base;
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
        private float _fixedY;
 
        public LinearMovementStrategy(SnakeConfig config)
        {
            _config = config;
        }
 
        public void Initialize(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
            _fixedY = rigidbody.position.y;
            PickRandomDirection();
 
            Observable
                .Interval(TimeSpan.FromSeconds(_config.DirectionChangeInterval))
                .Subscribe(_ => PickRandomDirection())
                .AddTo(_disposable);
        }
 
        public void Tick(Rigidbody rigidbody, Bounds bounds)
        {
            _bounds = bounds;
 
            var pos = rigidbody.position;
            if (!Mathf.Approximately(pos.y, _fixedY))
            {
                pos.y = _fixedY;
                rigidbody.position = pos;
            }
 
            var vel = rigidbody.linearVelocity;
            vel.y = 0f;
            rigidbody.linearVelocity = vel;
            rigidbody.angularVelocity = Vector3.zero;
 
            HandleBounds();
 
            rigidbody.MovePosition(new Vector3(
                rigidbody.position.x + _direction.x * _config.MoveSpeed * Time.fixedDeltaTime,
                _fixedY,
                rigidbody.position.z + _direction.z * _config.MoveSpeed * Time.fixedDeltaTime
            ));
        }
 
        private void PickRandomDirection()
        {
            var angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
            _direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;
        }
 
        private void HandleBounds()
        {
            if (_rigidbody == null) return;
 
            var radius = _config.BoundsRadius;
            var pos = _rigidbody.position;
 
            var minX = _bounds.min.x + radius;
            var maxX = _bounds.max.x - radius;
            var minZ = _bounds.min.z + radius;
            var maxZ = _bounds.max.z - radius;
 
            if (pos.x <= minX || pos.x >= maxX)
            {
                _direction.x = -_direction.x;
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
            }
 
            if (pos.z <= minZ || pos.z >= maxZ)
            {
                _direction.z = -_direction.z;
                pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
            }
 
            _rigidbody.position = pos;
        }
 
        public void OnCollision(Collision collision) { }
 
        public void Dispose() => _disposable.Dispose();
    }
}