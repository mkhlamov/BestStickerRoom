using System;
using UnityEngine;
using UnityEngine.InputSystem;
using BestStickerRoom.Core;
using Zenject;

namespace BestStickerRoom.Room
{
    public class WallDetector : IWallDetector, IInitializable, IDisposable
    {
        private const string WALL_TAG = "Wall";
        private const float MAX_RAYCAST_DISTANCE = 100f;
        private readonly LayerMask wallLayerMask = -1;

        private Camera raycastCamera;
        private InputManager inputManager;

        public event Action<WallHitResult> OnWallDetected;

        [Inject]
        public void Construct([Inject(Id = "RaycastCamera")] Camera camera, InputManager inputMgr)
        {
            raycastCamera = camera;
            inputManager = inputMgr;
        }

        public void Initialize()
        {
            if (inputManager != null)
            {
                inputManager.OnTap += HandleInput;
                inputManager.OnClick += HandleInput;
            }
        }

        public void Dispose()
        {
            if (inputManager != null)
            {
                inputManager.OnTap -= HandleInput;
                inputManager.OnClick -= HandleInput;
            }
        }

        private void HandleInput(Vector2 screenPosition)
        {
            Debug.Log($"HandleInput screenPosition {screenPosition:F2}");
            var hitResult = DetectWallFromScreenPosition(screenPosition);
            if (IsValidWallHit(hitResult))
            {
                OnWallDetected?.Invoke(hitResult);
            }
        }

        public WallHitResult DetectWallFromScreenPosition(Vector2 screenPosition)
        {
            if (raycastCamera == null)
            {
                return WallHitResult.Invalid;
            }

            Ray ray = raycastCamera.ScreenPointToRay(screenPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, MAX_RAYCAST_DISTANCE, wallLayerMask))
            {
                Debug.Log($"HandleInput hit {hit.collider.gameObject.name}");
                if (hit.collider.CompareTag(WALL_TAG))
                {
                    return WallHitResult.Create(hit);
                }
            }

            return WallHitResult.Invalid;
        }

        public WallHitResult DetectWallFromWorldRay(Ray worldRay)
        {
            if (Physics.Raycast(worldRay, out RaycastHit hit, MAX_RAYCAST_DISTANCE, wallLayerMask))
            {
                if (hit.collider.CompareTag(WALL_TAG))
                {
                    return WallHitResult.Create(hit);
                }
            }

            return WallHitResult.Invalid;
        }

        public WallHitResult DetectWallFromTouchOrMouse()
        {
            var pointer = Pointer.current;
            if (pointer != null)
            {
                return DetectWallFromScreenPosition(pointer.position.ReadValue());
            }

            var mouse = Mouse.current;
            if (mouse != null)
            {
                return DetectWallFromScreenPosition(mouse.position.ReadValue());
            }

            return WallHitResult.Invalid;
        }

        public bool IsValidWallHit(WallHitResult hitResult)
        {
            return hitResult.IsValid && hitResult.WallObject != null;
        }
    }
}

