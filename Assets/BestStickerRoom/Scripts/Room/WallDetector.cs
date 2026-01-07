using System;
using UnityEngine;
using UnityEngine.InputSystem;
using BestStickerRoom.Core;
using Zenject;

namespace BestStickerRoom.Room
{
    public class WallDetector : MonoBehaviour
    {
        [SerializeField] private string wallTag = "Wall";
        [SerializeField] private float maxRaycastDistance = 100f;
        [SerializeField] private LayerMask wallLayerMask = -1;

        private Camera raycastCamera;
        private InputManager inputManager;

        public event Action<WallHitResult> OnWallDetected;

        [Inject]
        private void Construct([Inject(Id = "RaycastCamera")] Camera camera, InputManager inputMgr)
        {
            raycastCamera = camera;
            inputManager = inputMgr;
        }

        private void OnEnable()
        {
            if (inputManager != null)
            {
                inputManager.OnTap += HandleInput;
                inputManager.OnClick += HandleInput;
            }
        }

        private void OnDisable()
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

            if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance, wallLayerMask))
            {
                Debug.Log($"HandleInput hit {hit.collider.gameObject.name}");
                if (hit.collider.CompareTag(wallTag))
                {
                    return WallHitResult.Create(hit);
                }
            }

            return WallHitResult.Invalid;
        }

        public WallHitResult DetectWallFromWorldRay(Ray worldRay)
        {
            if (Physics.Raycast(worldRay, out RaycastHit hit, maxRaycastDistance, wallLayerMask))
            {
                if (hit.collider.CompareTag(wallTag))
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

