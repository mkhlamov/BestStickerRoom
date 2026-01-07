using UnityEngine;
using Zenject;

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
        }
    }
}

