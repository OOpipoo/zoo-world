using _Project.Infrastructures.Services;
using UnityEngine;
using Zenject;

namespace _Project.Installers
{
	public class ProjectInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			BindScoreService();
		}
 
		private void BindScoreService()
		{
			Container
				.BindInterfacesAndSelfTo<ScoreService>()
				.AsSingle();
		}
	}
}