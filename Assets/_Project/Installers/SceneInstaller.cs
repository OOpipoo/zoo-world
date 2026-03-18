using System.Collections.Generic;
using _Project.Configs;
using _Project.Infrastructure.Factories;
using _Project.Infrastructure.Pools;
using _Project.Infrastructure.Services;
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
        [SerializeField] private TastyLabelPool _tastyLabelPool;
        [SerializeField] private HudView _hudView;
 
        [Header("Configs")]
        [SerializeField] private SpawnLimitsConfig _spawnLimitsConfig;
        [SerializeField] private List<AnimalConfig> _animalConfigs;
 
        public override void InstallBindings()
        {
            BindCamera();
            BindGameBoundsService();
            BindAnimalConfigs();
            BindSpawnLimits();
            BindTastyLabelPool();
            BindAnimalPool();
            BindAnimalRegistry();
            BindAnimalFactory();
            BindAnimalSpawnService();
            BindGameService();
            BindHudView();
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
 
        private void BindSpawnLimits()
        {
            Container
                .Bind<SpawnLimitsConfig>()
                .FromInstance(_spawnLimitsConfig)
                .AsSingle();
        }
 
        private void BindTastyLabelPool()
        {
            Container
                .Bind<TastyLabelPool>()
                .FromInstance(_tastyLabelPool)
                .AsSingle();
        }
 
        private void BindAnimalPool()
        {
            var animalPool = new AnimalPool(_animalConfigs);
            Container
                .Bind<AnimalPool>()
                .FromInstance(animalPool)
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
 
        private void BindGameService()
        {
            Container
                .Bind<GameService>()
                .AsSingle();
        }

        private void BindHudView()
        {
            Container
                .Bind<HudView>()
                .FromInstance(_hudView)
                .AsSingle();
        }
    }
}


