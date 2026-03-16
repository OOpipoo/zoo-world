using System.Collections.Generic;
using _Project.Animals;
using _Project.Animals.Base;

namespace _Project.Infrastructures.Services
{
	public class AnimalRegistry
	{
		private readonly Dictionary<AnimalView, AnimalPresenter> _registry
			= new Dictionary<AnimalView, AnimalPresenter>();
 
		public void Register(AnimalPresenter presenter, AnimalView view)
		{
			if (!_registry.ContainsKey(view))
				_registry.Add(view, presenter);
		}
 
		public void Unregister(AnimalView view)
		{
			if (_registry.ContainsKey(view))
				_registry.Remove(view);
		}
 
		public AnimalPresenter GetPresenter(AnimalView view)
		{
			_registry.TryGetValue(view, out var presenter);
			return presenter;
		}
 
		public IEnumerable<AnimalPresenter> GetAll() => _registry.Values;
	}
}