using GameCore.Controllers;
using GameCore.Entity.Factory;
using GameCore.Models;
using GameCore.Services;
using GameCore.Services.GameMechanicsService;
using GameCore.Services.GameTimerService;
using GameCore.Services.InputService;
using GameCore.Services.MazeControllerService;
using GameCore.Services.SceneControllerService;
using UI.HUD;
using UnityEngine;
using Zenject;

namespace GameCore.Installers
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        [SerializeField] private HUDView _hudView;
        [SerializeField] private SceneView _sceneView;

        public override void InstallBindings()
        {
            //game session model initialization
            Container.Bind<GameSessionModel>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<GameSessionService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameMechanicsService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
            Container.BindInterfacesAndSelfTo<MazeControllerService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameTimerService>().AsSingle().NonLazy();

            //HUD MVC entry point
            var hudView = GameObject.Instantiate(_hudView);
            Container.Bind<HUDView>().FromInstance(hudView).AsSingle();
            Container.BindInterfacesAndSelfTo<HUDController>().AsSingle().NonLazy();
            
            var sceneView = GameObject.Instantiate(_sceneView);
            Container.BindInterfacesAndSelfTo<SceneControllerService>().AsSingle();
            Container.Bind<ISceneView>().FromInstance(sceneView.GetComponent<SceneView>()).AsSingle().WhenInjectedInto<SceneControllerService>();
            
            Container.BindInterfacesTo<PlayerFactory>().AsSingle().WhenInjectedInto<GameMechanicsService>();
        }
    }
}