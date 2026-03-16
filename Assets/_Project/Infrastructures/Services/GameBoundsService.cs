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
			CalculateBounds();
		}
		
		private void CalculateBounds()
		{
			var cam = _camera;
			var height = cam.orthographicSize;
			var width = height * cam.aspect;
 
			var center = new Vector3(cam.transform.position.x, 0f, cam.transform.position.z);
 
			Bounds = new Bounds(center, new Vector3(width * 2f, 0f, height * 2f));
		}
	}
}