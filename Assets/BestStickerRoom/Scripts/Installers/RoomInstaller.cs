using UnityEngine;
using Zenject;
using BestStickerRoom.Room;
using BestStickerRoom.Data;

namespace BestStickerRoom.Installers
{
    public class RoomInstaller : MonoInstaller
    {
        [SerializeField] private Camera raycastCamera;
        [SerializeField] private RoomHitValidator roomHitValidator;

        public override void InstallBindings()
        {
            if (raycastCamera == null)
            {
                Debug.LogError("RoomInstaller: RaycastCamera is not assigned. Please assign a camera in the inspector.");
                return;
            }

            if (roomHitValidator == null)
            {
                Debug.LogError("RoomInstaller: RoomHitValidator is not assigned. Please assign it in the inspector.");
                return;
            }

            Container.Bind<Camera>()
                .WithId("RaycastCamera")
                .FromInstance(raycastCamera)
                .AsSingle();

            Container.Bind<RoomHitValidator>()
                .FromInstance(roomHitValidator)
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

