using System;
using UniRx;
using UnityEngine;
using _Project.Animals.Abstractions;
using _Project.Configs;
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
        private bool _isStunned;
        private float _jumpProgress;
        private float _fixedY;
        private Vector3 _jumpStart;
        private Vector3 _jumpEnd;
 
        private const float StunDuration = 0.4f;
        private const float CollisionForce = 4f;
 
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
 
            rigidbody.angularVelocity = Vector3.zero;
 
            if (_isStunned)
            {
                var vel = rigidbody.linearVelocity;
                vel.y = 0f;
                rigidbody.linearVelocity = vel;
 
                var pos = rigidbody.position;
                pos.y = _fixedY;
                rigidbody.position = pos;
                return;
            }
 
            if (!_isJumping)
            {
                rigidbody.linearVelocity = Vector3.zero;
                var pos = rigidbody.position;
                pos.y = _fixedY;
                rigidbody.position = pos;
                return;
            }
 
            _jumpProgress += Time.fixedDeltaTime / _config.JumpDuration;
            _jumpProgress = Mathf.Clamp01(_jumpProgress);
 
            var currentPos = rigidbody.position;
 
            if (IsOutOfBounds(currentPos))
            {
                ReflectDirection(currentPos);
                var clamped = ClampToBounds(currentPos);
                _jumpStart = new Vector3(clamped.x, 0f, clamped.z);
                _jumpEnd = _jumpStart + _direction * _config.JumpDistance;
                _jumpProgress = 0f;
            }
 
            var targetXZ = Vector3.Lerp(
                new Vector3(_jumpStart.x, 0f, _jumpStart.z),
                new Vector3(_jumpEnd.x, 0f, _jumpEnd.z),
                _jumpProgress
            );
 
            var xzDiff = targetXZ - new Vector3(currentPos.x, 0f, currentPos.z);
            rigidbody.linearVelocity = new Vector3(
                xzDiff.x / Time.fixedDeltaTime,
                0f,
                xzDiff.z / Time.fixedDeltaTime
            );
 
            var yOffset = Mathf.Sin(_jumpProgress * Mathf.PI) * _config.JumpHeight;
            rigidbody.MovePosition(new Vector3(currentPos.x, _fixedY + yOffset, currentPos.z));
 
            if (_direction != Vector3.zero)
                rigidbody.transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
 
            if (_jumpProgress >= 1f)
            {
                _isJumping = false;
                rigidbody.linearVelocity = Vector3.zero;
            }
        }
 
        public void OnCollision(Collision collision)
        {
            if (_rigidbody == null || _isStunned) return;
 
            _isJumping = false;
            _isStunned = true;
 
            var contactNormal = collision.contacts[0].normal;
            var bounceDir = new Vector3(contactNormal.x, 0f, contactNormal.z).normalized;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.AddForce(bounceDir * CollisionForce, ForceMode.Impulse);
 
            Observable
                .Timer(TimeSpan.FromSeconds(StunDuration))
                .Subscribe(_ =>
                {
                    _isStunned = false;
                    if (_rigidbody != null)
                    {
                        var pos = _rigidbody.position;
                        pos.y = _fixedY;
                        _rigidbody.position = pos;
                        _rigidbody.linearVelocity = Vector3.zero;
                    }
                })
                .AddTo(_disposable);
        }
 
        private void TryStartJump()
        {
            if (_rigidbody == null || _isJumping || _isStunned) return;
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
 
        private Vector3 ClampToBounds(Vector3 pos)
        {
            var r = _config.BoundsRadius;
            return new Vector3(
                Mathf.Clamp(pos.x, _bounds.min.x + r, _bounds.max.x - r),
                pos.y,
                Mathf.Clamp(pos.z, _bounds.min.z + r, _bounds.max.z - r)
            );
        }
 
        private void PickRandomDirection()
        {
            var angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            _direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;
        }
 
        public void Dispose() => _disposable.Dispose();
    }
}