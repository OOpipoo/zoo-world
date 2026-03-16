using System;
using UniRx;
using _Project.Infrastructures.Services;
using _Project.Ui;

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
 
            var otherPresenter = _registry.GetPresenter(otherView);
            if (otherPresenter == null) return;
 
            var otherWasAlive = otherPresenter.Animal.IsAlive;
            _collisionHandler.HandleCollision(_model, otherPresenter.Animal);
 
            if (!_model.IsPrey && otherWasAlive && !otherPresenter.Animal.IsAlive)
                _tastyLabel?.Show(_view.transform);
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
            _movementStrategy.Dispose();
            _disposable.Dispose();
        }
    }
}
 