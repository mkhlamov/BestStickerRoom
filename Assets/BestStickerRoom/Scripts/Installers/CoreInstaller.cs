using BestStickerRoom.Core;
using BestStickerRoom.UI;
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

            Container.Bind<DragDropHandler>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<DragDropScrollRectGate>()
                .AsSingle()
                .NonLazy();
        }
    }
}

