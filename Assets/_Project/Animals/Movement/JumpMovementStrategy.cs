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
        private Vector3 _direction;
        private Bounds _bounds;
 
        private bool _isJumping;
        private float _jumpProgress;
        private Vector3 _jumpStart;
        private Vector3 _jumpEnd;
        private float _fixedY;
 
        public JumpMovementStrategy(FrogConfig config)
        {
            _config = config;
        }
 
        public void Initialize(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
            _fixedY = rigidbody.position.y;
            PickRandomDirection();
 
            Observable
                .Interval(TimeSpan.FromSeconds(_config.JumpInterval))
                .Subscribe(_ => TryStartJump())
                .AddTo(_disposable);
        }
 
        public void Tick(Rigidbody rigidbody, Bounds bounds)
        {
            _bounds = bounds;
 
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
 
            if (!_isJumping) return;
 
            _jumpProgress += Time.fixedDeltaTime / _config.JumpDuration;
            _jumpProgress = Mathf.Clamp01(_jumpProgress);
 
            var xz = Vector3.Lerp(_jumpStart, _jumpEnd, _jumpProgress);
            var yOffset = Mathf.Sin(_jumpProgress * Mathf.PI) * _config.JumpHeight;
            var nextPos = new Vector3(xz.x, _fixedY + yOffset, xz.z);
 
            if (IsOutOfBounds(nextPos))
            {
                ReflectDirection(nextPos);
 
                var currentXZ = new Vector3(
                    Mathf.Clamp(nextPos.x, _bounds.min.x + _config.BoundsRadius, _bounds.max.x - _config.BoundsRadius),
                    0f,
                    Mathf.Clamp(nextPos.z, _bounds.min.z + _config.BoundsRadius, _bounds.max.z - _config.BoundsRadius)
                );
 
                _jumpStart = currentXZ;
                _jumpEnd = currentXZ + _direction * _config.JumpDistance;
                _jumpProgress = 0f;
 
                nextPos = new Vector3(currentXZ.x, _fixedY, currentXZ.z);
            }
 
            rigidbody.MovePosition(nextPos);
 
            if (_direction != Vector3.zero)
                rigidbody.transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
 
            if (_jumpProgress >= 1f)
                _isJumping = false;
        }
 
        private bool IsOutOfBounds(Vector3 pos)
        {
            var r = _config.BoundsRadius;
            return pos.x < _bounds.min.x + r
                || pos.x > _bounds.max.x - r
                || pos.z < _bounds.min.z + r
                || pos.z > _bounds.max.z - r;
        }
 
        private void ReflectDirection(Vector3 pos)
        {
            var r = _config.BoundsRadius;
 
            if (pos.x < _bounds.min.x + r || pos.x > _bounds.max.x - r)
                _direction.x = -_direction.x;
 
            if (pos.z < _bounds.min.z + r || pos.z > _bounds.max.z - r)
                _direction.z = -_direction.z;
 
            _direction.Normalize();
        }
 
        private void TryStartJump()
        {
            if (_rigidbody == null || _isJumping) return;
 
            PickRandomDirection();
            StartJump();
        }
 
        private void StartJump()
        {
            _jumpStart = new Vector3(_rigidbody.position.x, 0f, _rigidbody.position.z);
            _jumpEnd = _jumpStart + _direction * _config.JumpDistance;
            _jumpProgress = 0f;
            _isJumping = true;
        }
 
        private void PickRandomDirection()
        {
            var angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            _direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;
        }
 
        public void Dispose() => _disposable.Dispose();
    }
}