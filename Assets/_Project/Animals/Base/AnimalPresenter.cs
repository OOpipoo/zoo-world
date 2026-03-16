using System;
using UniRx;
using _Project.Infrastructures.Services;

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
 
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
 
        public IAnimal Animal => _model;
 
        public AnimalPresenter(
            AnimalModel model,
            AnimalView view,
            IMovementStrategy movementStrategy,
            ICollisionHandler collisionHandler,
            GameBoundsService boundsService,
            AnimalRegistry registry)
        {
            _model = model;
            _view = view;
            _movementStrategy = movementStrategy;
            _collisionHandler = collisionHandler;
            _boundsService = boundsService;
            _registry = registry;
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
 
            _collisionHandler.HandleCollision(_model, otherPresenter.Animal);
        }
 
        private void OnDied()
        {
            _registry.Unregister(_view);
            _view.Destroy();
            Dispose();
        }
 
        public void Dispose()
        {
            _view.OnFixedUpdated -= OnFixedUpdate;
            _view.OnCollisionEntered -= OnCollisionEntered;
            _disposable.Dispose();
        }
    }
}