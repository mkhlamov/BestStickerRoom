using System;
using UnityEngine;
using UnityEngine.InputSystem;
using BestStickerRoom.Core;
using Zenject;

namespace BestStickerRoom.Room
{
    public class WallDetector : IWallDetector, IInitializable, IDisposable
    {
        private const string WALL_TAG = "Room";
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

            var worldPoint = raycastCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0f));
            var hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f, wallLayerMask);

            if (hit.collider != null && hit.collider.CompareTag(WALL_TAG))
            {
                return WallHitResult.Create(hit);
            }

            return WallHitResult.Invalid;
        }

        public WallHitResult DetectWallFromWorldRay(Ray worldRay)
        {
            var origin = new Vector2(worldRay.origin.x, worldRay.origin.y);
            var direction = new Vector2(worldRay.direction.x, worldRay.direction.y).normalized;
            
            var hit = Physics2D.Raycast(origin, direction, MAX_RAYCAST_DISTANCE, wallLayerMask);

            if (hit.collider != null && hit.collider.CompareTag(WALL_TAG))
            {
                return WallHitResult.Create(hit);
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

