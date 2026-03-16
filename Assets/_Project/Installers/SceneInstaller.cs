using System.Collections.Generic;
using _Project.Configs;
using _Project.Infrastructures.Factories;
using _Project.Infrastructures.Services;
using UnityEngine;
using Zenject;

namespace _Project.Installers
{
	public class SceneInstaller : MonoInstaller
	{
		[SerializeField] private List<AnimalConfig> _animalConfigs;
 
		public override void InstallBindings()
		{
			BindAnimalConfigs();
			BindAnimalRegistry();
			BindAnimalFactory();
			BindAnimalSpawnService();
		}
 
		private void BindAnimalConfigs()
		{
			Container
				.Bind<List<AnimalConfig>>()
				.FromInstance(_animalConfigs)
				.AsSingle();
		}
 
		private void BindAnimalRegistry()
		{
			Container
				.Bind<AnimalRegistry>()
				.AsSingle();
		}
 
		private void BindAnimalFactory()
		{
			Container
				.Bind<AnimalFactory>()
				.AsSingle();
		}
 
		private void BindAnimalSpawnService()
		{
			Container
				.BindInterfacesAndSelfTo<AnimalSpawnService>()
				.AsSingle();
		}
	}
}