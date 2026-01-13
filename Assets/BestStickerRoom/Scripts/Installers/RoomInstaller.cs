using UnityEngine;
using Zenject;
using BestStickerRoom.Room;
using BestStickerRoom.Data;

namespace BestStickerRoom.Installers
{
    public class RoomInstaller : MonoInstaller
    {
        [SerializeField] private Camera raycastCamera;

        public override void InstallBindings()
        {
            if (raycastCamera == null)
            {
                Debug.LogError("RoomInstaller: RaycastCamera is not assigned. Please assign a camera in the inspector.");
                return;
            }

            Container.Bind<Camera>()
                .WithId("RaycastCamera")
                .FromInstance(raycastCamera)
                .AsSingle();

            Container.BindInterfacesAndSelfTo<WallDetector>()
                .AsSingle()
                .NonLazy();

            Container.Bind<StickerPlacer>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }
    }
}

