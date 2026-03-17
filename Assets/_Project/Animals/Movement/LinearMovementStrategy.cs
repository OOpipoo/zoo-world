using System;
using UniRx;
using UnityEngine;
using _Project.Animals.Abstractions;
using _Project.Configs;

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
 
            var vel = rigidbody.linearVelocity;
            vel.x = Mathf.Lerp(vel.x, 0f, _config.VelocityDamping * Time.fixedDeltaTime);
            vel.z = Mathf.Lerp(vel.z, 0f, _config.VelocityDamping * Time.fixedDeltaTime);
            vel.y = 0f;
            rigidbody.linearVelocity = vel;
 
            var pos = rigidbody.position;
            if (!Mathf.Approximately(pos.y, _fixedY))
            {
                pos.y = _fixedY;
                rigidbody.position = pos;
            }
 
            HandleBounds();
 
            if (_direction != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(_direction, Vector3.up);
                rigidbody.transform.rotation = Quaternion.Slerp(
                    rigidbody.transform.rotation,
                    targetRotation,
                    _config.RotationSpeed * Time.fixedDeltaTime
                );
            }
 
            rigidbody.MovePosition(new Vector3(
                rigidbody.position.x + _direction.x * _config.MoveSpeed * Time.fixedDeltaTime,
                _fixedY,
                rigidbody.position.z + _direction.z * _config.MoveSpeed * Time.fixedDeltaTime
            ));
        }
 
        public void OnCollision(Collision collision) { }
 
        private void PickRandomDirection()
        {
            var currentAngle = Mathf.Atan2(_direction.z, _direction.x) * Mathf.Rad2Deg;
            var maxTurn = _config.MaxTurnAngle;
            var randomOffset = UnityEngine.Random.Range(-maxTurn, maxTurn);
            var newAngle = (currentAngle + randomOffset) * Mathf.Deg2Rad;
 
            _direction = new Vector3(
                Mathf.Cos(newAngle),
                0f,
                Mathf.Sin(newAngle)
            ).normalized;
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
 
            _rigidbody.position = new Vector3(pos.x, _fixedY, pos.z);
        }
 
        public void Dispose() => _disposable.Dispose();
    }
}