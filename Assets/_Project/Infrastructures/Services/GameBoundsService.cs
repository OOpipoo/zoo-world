using UnityEngine;
using Zenject;

namespace _Project.Infrastructures.Services
{
	public class GameBoundsService : IInitializable
	{
		private readonly Camera _camera;
 
		public Bounds Bounds { get; private set; }
 
		public GameBoundsService(Camera camera)
		{
			_camera = camera;
		}
 
		public void Initialize()
		{
			var height = _camera.orthographicSize;
			var width = height * _camera.aspect;
			var center = new Vector3(
				_camera.transform.position.x,
				0f,
				_camera.transform.position.z
			);
 
			Bounds = new Bounds(center, new Vector3(width * 2f, 0f, height * 2f));
		}
	}
}
