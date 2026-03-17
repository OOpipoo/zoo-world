using System;
using _Project.Animals.Abstractions;
using _Project.Infrastructure.Pools;
using _Project.Infrastructure.Services;
using _Project.Infrastructures.Services;
using _Project.Ui;
using UniRx;
using UnityEngine;

namespace _Project.Animals.Core
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
        private readonly AnimalPool _animalPool;
        private readonly TastyLabelPool _tastyLabelPool;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private TastyLabel _currentLabel;
        private IDisposable _labelTimer;

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
            AnimalPool animalPool,
            TastyLabelPool tastyLabelPool = null)
        {
            _model = model;
            _view = view;
            _movementStrategy = movementStrategy;
            _collisionHandler = collisionHandler;
            _boundsService = boundsService;
            _registry = registry;
            _scoreService = scoreService;
            _animalPool = animalPool;
            _tastyLabelPool = tastyLabelPool;
        }

        public void Initialize()
        {
            _view.IsPrey = _model.IsPrey;
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

        private void OnFixedUpdate()
        {
            if (!_model.IsAlive) return;
            _movementStrategy.Tick(_view.Rigidbody, _boundsService.Bounds);
        }

        private void OnCollisionEntered(AnimalView otherView)
        {
            if (!_model.IsAlive) return;
            if (_model.IsPrey) return;

            var otherPresenter = _registry.GetPresenter(otherView);

            if (otherPresenter == null)
            {
                if (!otherView.IsPrey && _model.IsAlive && !otherView.gameObject.activeInHierarchy)
                    ShowKillLabel();
                return;
            }

            if (otherPresenter.Animal.IsPrey && !IsHeadCollisionXZ(otherView)) return;

            _collisionHandler.HandleCollision(_model, otherPresenter.Animal);

            if (_model.IsAlive && !otherPresenter.Animal.IsAlive)
                ShowKillLabel();
        }

        private void OnCollisionPhysics(Collision collision)
        {
            if (!_model.IsAlive) return;
            _movementStrategy.OnCollision(collision);
        }

        private void ShowKillLabel()
        {
            if (_tastyLabelPool == null) return;

            _labelTimer?.Dispose();

            if (_currentLabel == null)
                _currentLabel = _tastyLabelPool.Get();

            if (_currentLabel == null) return;

            _currentLabel.Show(_view.TastyLabelAnchor);

            _labelTimer = Observable
                .Timer(TimeSpan.FromSeconds(_tastyLabelPool.DisplayDuration))
                .Subscribe(_ => HideKillLabel());
        }

        private void HideKillLabel()
        {
            _labelTimer?.Dispose();
            _labelTimer = null;

            if (_currentLabel == null) return;
            _tastyLabelPool?.Return(_currentLabel);
            _currentLabel = null;
        }

        private bool IsHeadCollisionXZ(AnimalView otherView)
        {
            var toOther = otherView.transform.position - _view.transform.position;
            toOther.y = 0f;
            if (toOther.sqrMagnitude < 0.001f) return true;

            var forward = _view.transform.forward;
            forward.y = 0f;

            return Vector3.Dot(forward.normalized, toOther.normalized) > 0.2f;
        }

        private void OnDied()
        {
            HideKillLabel();

            if (_model.IsPrey)
                _scoreService.AddPreyDeath();
            else
                _scoreService.AddPredatorDeath();

            _registry.Unregister(_view);
            _animalPool.Return(_view, _model.AnimalType);
            Dispose();
        }

        public void Dispose()
        {
            HideKillLabel();

            _view.OnFixedUpdated -= OnFixedUpdate;
            _view.OnCollisionEntered -= OnCollisionEntered;
            _view.OnCollisionPhysics -= OnCollisionPhysics;
            _movementStrategy.Dispose();
            _disposable.Dispose();
        }
    }
}
