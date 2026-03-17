using _Project.Infrastructures.Services;
using Zenject;

namespace _Project.Installers
{
	public class ProjectInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container
				.Bind<ScoreService>()
				.AsSingle();
		}
	}
}
