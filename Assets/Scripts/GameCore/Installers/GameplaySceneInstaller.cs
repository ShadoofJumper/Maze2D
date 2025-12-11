using UnityEngine;
using Zenject;

namespace GameCore.Installers
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _hudView;
        
        public override void InstallBindings()
        {
            
        }
    }
}