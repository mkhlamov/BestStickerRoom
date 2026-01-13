using System;
using UnityEngine;
using BestStickerRoom.Core;
using BestStickerRoom.Data;
using BestStickerRoom.UI;
using Zenject;

namespace BestStickerRoom.Room
{
    public class StickerPlacer : MonoBehaviour
    {
        [SerializeField] private float followDistance = 2f;

        private LevelSettings levelSettings;
        private DragDropHandler dragDropHandler;
        private Camera raycastCamera;
        private Transform stickerParent;

        [SerializeField] private GameObject currentStickerInstance;
        private DragDropData currentDragData;

        public event Action<GameObject> OnStickerPlaced;

        [Inject]
        private void Construct(DragDropHandler dragDrop, LevelSettings settings, [Inject(Id = "RaycastCamera")] Camera camera)
        {
            dragDropHandler = dragDrop;
            levelSettings = settings;
            raycastCamera = camera;
        }

        private void Awake()
        {
            if (levelSettings == null)
            {
                Debug.LogError("StickerPlacer: LevelSettings is not assigned!");
            }
        }

        private void OnEnable()
        {
            if (dragDropHandler != null)
            {
                dragDropHandler.OnDragStarted += HandleDragStarted;
                dragDropHandler.OnDragUpdated += HandleDragUpdated;
                dragDropHandler.OnDragDropped += HandleDragDropped;
                dragDropHandler.OnDragCancelled += HandleDragCancelled;
            }
        }

        private void OnDisable()
        {
            if (dragDropHandler != null)
            {
                dragDropHandler.OnDragStarted -= HandleDragStarted;
                dragDropHandler.OnDragUpdated -= HandleDragUpdated;
                dragDropHandler.OnDragDropped -= HandleDragDropped;
                dragDropHandler.OnDragCancelled -= HandleDragCancelled;
            }
        }

        private void HandleDragStarted(DragDropData dragData)
        {
            if (levelSettings == null || levelSettings.StickerPrefab == null)
            {
                Debug.LogError("StickerPlacer: LevelSettings or StickerPrefab is null!");
                return;
            }

            currentDragData = dragData;
            CreateStickerInstance();
            ApplyStickerData(currentStickerInstance, dragData);
        }

        private void HandleDragUpdated(DragDropData dragData)
        {
            if (currentStickerInstance == null) return;
            UpdateStickerPosition(dragData.CurrentScreenPosition);
        }

        private void HandleDragDropped(DragDropData dragData, WallHitResult wallHit)
        {
            if (currentStickerInstance == null) return;
            
            UpdateStickerPosition(dragData.CurrentScreenPosition);
            OnStickerPlaced?.Invoke(currentStickerInstance);

            currentStickerInstance = null;
            currentDragData = null;
        }

        private void HandleDragCancelled(DragDropData dragData)
        {
            Debug.Log("HandleDragCancelled");
            if (currentStickerInstance != null)
            {
                DestroyStickerInstance();
                currentStickerInstance = null;
            }

            currentDragData = null;
        }

        private void CreateStickerInstance()
        {
            if (raycastCamera == null)
            {
                Debug.LogError("StickerPlacer: RaycastCamera is not assigned!");
                return;
            }

            var screenPosition = currentDragData.CurrentScreenPosition;
            var stickerPrefab = levelSettings.StickerPrefab;

            var position = GetStickerPosition(screenPosition);
            var rotation = Quaternion.identity;

            currentStickerInstance = Instantiate(stickerPrefab, position, rotation);

            if (stickerParent == null)
            {
                stickerParent = new GameObject("Stickers").transform;
            }

            currentStickerInstance.transform.SetParent(stickerParent);

            var stickerTransform = currentStickerInstance.transform;
            stickerTransform.localScale = new Vector3(
                levelSettings.StickerSize.x,
                levelSettings.StickerSize.y,
                1f
            );
        }

        private void UpdateStickerPosition(Vector2 screenPosition)
        {
            if (currentStickerInstance == null || raycastCamera == null) return;

            var position = GetStickerPosition(screenPosition);
            currentStickerInstance.transform.position = position;
        }

        private Vector3 GetStickerPosition(Vector2 screenPosition)
        {
            if (raycastCamera == null)
            {
                return Vector3.zero;
            }

            if (!raycastCamera.orthographic)
            {
                Debug.LogError("StickerPlacer: Orthographic camera is required for 2D room!");
                return Vector3.zero;
            }

            var zDepth = raycastCamera.transform.position.z + levelSettings.StickerOffsetFromSurface;
            var worldPos = raycastCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, zDepth));
            return worldPos;
        }

        private void DestroyStickerInstance()
        {
            if (currentStickerInstance != null)
            {
                Destroy(currentStickerInstance);
                currentStickerInstance = null;
            }
        }

        private void ApplyStickerData(GameObject stickerInstance, DragDropData dragData)
        {
            if (dragData?.Data is StickerData stickerData && stickerData.Asset != null)
            {
                var spriteRenderer = stickerInstance.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null && stickerData.Sprite != null)
                {
                    spriteRenderer.sprite = stickerData.Sprite;
                }
            }
        }
    }
}
