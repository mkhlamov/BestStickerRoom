using BestStickerRoom.Core;
using UnityEngine;
using Zenject;

namespace BestStickerRoom.Installers
{
    public class CoreInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
        
            Container.Bind<InputManager>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }
    }
}

