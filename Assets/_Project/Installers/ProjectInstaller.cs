using _Project.Infrastructures.Services;
using UnityEngine;
using Zenject;

namespace _Project.Installers
{
	public class ProjectInstaller : MonoInstaller
	{
		[SerializeField] private Camera _mainCamera;
 
		public override void InstallBindings()
		{
			BindCamera();
			BindGameBoundsService();
			BindScoreService();
		}
		
		private void BindCamera()
		{
			Container
				.Bind<Camera>()
				.FromInstance(_mainCamera)
				.AsSingle();
		}
 
		private void BindGameBoundsService()
		{
			Container
				.BindInterfacesAndSelfTo<GameBoundsService>()
				.AsSingle();
		}
 
		private void BindScoreService()
		{
			Container
				.BindInterfacesAndSelfTo<ScoreService>()
				.AsSingle();
		}
	}
}