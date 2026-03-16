using System.Collections.Generic;
using _Project.Configs;
using _Project.Infrastructures.Factories;
using _Project.Infrastructures.Services;
using _Project.Ui;
using UnityEngine;
using Zenject;

namespace _Project.Installers
{
	public class SceneInstaller : MonoInstaller
	{
		[Header("Scene References")]
		[SerializeField] private Camera _mainCamera;
		[SerializeField] private TastyLabel _tastyLabelPrefab;
 
		[Header("Animal Configs")]
		[SerializeField] private List<AnimalConfig> _animalConfigs;
 
		
		public override void InstallBindings()
		{
			BindCamera();
			BindGameBoundsService();
			BindAnimalConfigs();
			BindTastyLabel();
			BindAnimalRegistry();
			BindAnimalFactory();
			BindAnimalSpawnService();
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
 
		private void BindAnimalConfigs()
		{
			Container
				.Bind<List<AnimalConfig>>()
				.FromInstance(_animalConfigs)
				.AsSingle();
		}
 
		private void BindTastyLabel()
		{
			Container
				.Bind<TastyLabel>()
				.FromInstance(_tastyLabelPrefab)
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

