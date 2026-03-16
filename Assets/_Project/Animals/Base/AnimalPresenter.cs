using System;
using UniRx;
using _Project.Infrastructures.Services;
using _Project.Ui;
using UnityEngine;

namespace _Project.Animals.Base
{
	 public class AnimalPresenter : IDisposable
    {
        private readonly AnimalModel _model;
        private readonly AnimalView _view;
        private readonly IMovementStrategy _movementStrategy;
        private readonly ICollisionHandler _collisionHandler;
        private readonly GameBoundsService _boundsService;
        private readonly AnimalRegistry _registry;
        private readonly ScoreService _scoreService;
        private readonly TastyLabel _tastyLabel;
 
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
 
        public IAnimal Animal => _model;
        public AnimalView View => _view;
 
        public AnimalPresenter(
            AnimalModel model,
            AnimalView view,
            IMovementStrategy movementStrategy,
            ICollisionHandler collisionHandler,
            GameBoundsService boundsService,
            AnimalRegistry registry,
            ScoreService scoreService,
            TastyLabel tastyLabel = null)
        {
            _model = model;
            _view = view;
            _movementStrategy = movementStrategy;
            _collisionHandler = collisionHandler;
            _boundsService = boundsService;
            _registry = registry;
            _scoreService = scoreService;
            _tastyLabel = tastyLabel;
        }
 
        public void Initialize()
        {
            _movementStrategy.Initialize(_view.Rigidbody);
 
            _view.OnFixedUpdated += OnFixedUpdate;
            _view.OnCollisionEntered += OnCollisionEntered;
            _view.OnCollisionPhysics += OnCollisionPhysics;
 
            _model.IsAliveProperty
                .Where(isAlive => !isAlive)
                .Subscribe(_ => OnDied())
                .AddTo(_disposable);
 
            _registry.Register(this, _view);
        }
 
        private void OnCollisionPhysics(Collision collision)
        {
            if (!_model.IsAlive) return;
            _movementStrategy.OnCollision(collision);
        }
 
        private void OnFixedUpdate()
        {
            if (!_model.IsAlive) return;
            _movementStrategy.Tick(_view.Rigidbody, _boundsService.Bounds);
        }
 
        private void OnCollisionEntered(AnimalView otherView)
        {
            if (!_model.IsAlive) return;
 
            var otherPresenter = _registry.GetPresenter(otherView);
            if (otherPresenter == null) return;
 
            if (!_model.IsPrey && !IsHeadCollision(otherView))
                return;
 
            if (_model.IsPrey && !otherPresenter.Animal.IsPrey && !IsPredatorFacingMe(otherView))
                return;
 
            var otherWasAlive = otherPresenter.Animal.IsAlive;
            _collisionHandler.HandleCollision(_model, otherPresenter.Animal);
 
            if (!_model.IsPrey && otherWasAlive && !otherPresenter.Animal.IsAlive)
                _tastyLabel?.Show(_view.transform);
        }
 
        private bool IsPredatorFacingMe(AnimalView predatorView)
        {
            var toMe = (_view.transform.position - predatorView.transform.position).normalized;
            var predatorForward = predatorView.transform.forward;
            return Vector3.Dot(predatorForward, toMe) > 0.2f;
        }
        
        private bool IsHeadCollision(AnimalView otherView)
        {
            var toOther = (otherView.transform.position - _view.transform.position).normalized;
            var forward = _view.transform.forward;
            return Vector3.Dot(forward, toOther) > 0.2f;
        }
 
        private void OnDied()
        {
            if (_model.IsPrey)
                _scoreService.AddPreyKill();
            else
                _scoreService.AddPredatorKill();
 
            _registry.Unregister(_view);
            _view.Destroy();
            Dispose();
        }
 
        public void Dispose()
        {
            _view.OnFixedUpdated -= OnFixedUpdate;
            _view.OnCollisionEntered -= OnCollisionEntered;
            _view.OnCollisionPhysics -= OnCollisionPhysics;
            _movementStrategy.Dispose();
            _disposable.Dispose();
        }
    }
}
 
 