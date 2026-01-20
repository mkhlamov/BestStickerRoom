using System;
using UnityEngine;
using BestStickerRoom.Core;
using BestStickerRoom.Data;
using BestStickerRoom.UI;
using UniRx;
using Zenject;

namespace BestStickerRoom.Room
{
    public class StickerPlacer : MonoBehaviour
    {
        [SerializeField] private float sortingMultiplier = 100f;
        [SerializeField] private int baseSortingOrder = 0;
        [SerializeField] private string sortingLayerName = "Stickers";
        [SerializeField] private Sticker currentSticker;

        private LevelSettings levelSettings;
        private DragDropHandler dragDropHandler;
        private IDragDropGate dragDropGate;
        private Camera raycastCamera;
        private Room room;
        
        private Transform stickerParent;
        private DragDropData currentDragData;

        public event Action<Sticker> OnStickerPlaced;

        [Inject]
        private void Construct(
            DragDropHandler dragDrop,
            LevelSettings settings,
            IDragDropGate dragGate,
            [Inject(Id = "RaycastCamera")] Camera camera,
            Room roomInstance)
        {
            dragDropHandler = dragDrop;
            levelSettings = settings;
            dragDropGate = dragGate;
            raycastCamera = camera;
            room = roomInstance;
        }

        private void Awake()
        {
            if (levelSettings == null)
            {
                Debug.LogError("StickerPlacer: LevelSettings is not assigned!");
            }

            dragDropGate.DragAllowed
                .Subscribe(allowed =>
                {
                    if (!allowed) return;
                    if (currentDragData == null) return;
                    CreateStickerInstance();
                    ApplyStickerData(currentSticker, currentDragData);
                })
                .AddTo(this);
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
        }

        private void HandleDragUpdated(DragDropData dragData)
        {
            if (dragDropGate != null && !dragDropGate.DragAllowed.Value)
            {
                return;
            }
            if (currentSticker == null)
            {
                return;
            }
            UpdateStickerPosition(dragData.CurrentScreenPosition);
        }

        private void HandleDragDropped(DragDropData dragData, WallHitResult wallHit)
        {
            if (currentSticker == null) return;
            
            UpdateStickerPosition(dragData.CurrentScreenPosition);
            if (!FitsInRoomCollider(currentSticker, wallHit))
            {
                DestroyStickerInstance();
                currentDragData = null;
                return;
            }
            OnStickerPlaced?.Invoke(currentSticker);

            currentSticker = null;
            currentDragData = null;
        }

        private void HandleDragCancelled(DragDropData dragData)
        {
            if (currentSticker != null)
            {
                DestroyStickerInstance();
                currentSticker = null;
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

            var instance = Instantiate(stickerPrefab, position, rotation);
            var stickerInstance = instance.GetComponent<Sticker>();
            if(stickerInstance == null)
            {
                Debug.LogError("StickerPlacer: Sticker component is not assigned!");
                return;
            }
            currentSticker = stickerInstance;

            if (stickerParent == null)
            {
                stickerParent = new GameObject("Stickers").transform;
            }

            currentSticker.transform.SetParent(stickerParent);

            var stickerTransform = currentSticker.transform;
            stickerTransform.localScale = new Vector3(
                levelSettings.StickerSize.x,
                levelSettings.StickerSize.y,
                1f
            );
        }

        private void UpdateStickerPosition(Vector2 screenPosition)
        {
            if (currentSticker == null || raycastCamera == null) return;

            var position = GetStickerPosition(screenPosition);
            currentSticker.transform.position = position;
            UpdateStickerSorting(currentSticker, position);
        }

        private Vector3 GetStickerPosition(Vector2 screenPosition)
        {
            if (raycastCamera == null)
            {
                return Vector3.zero;
            }

            var worldPos = IsometricUtils.ScreenToIsometricWorld(screenPosition, raycastCamera);
            return worldPos;
        }

        private void UpdateStickerSorting(Sticker sticker, Vector3 worldPosition)
        {
            if (sticker == null) return;

            var spriteRenderer = sticker.SpriteRenderer;
            if (spriteRenderer == null) return;

            spriteRenderer.sortingLayerName = sortingLayerName;
            IsometricUtils.UpdateSortingOrder(spriteRenderer, worldPosition, baseSortingOrder, sortingMultiplier);
        }

        private bool FitsInRoomCollider(Sticker sticker, WallHitResult wallHit)
        {
            if (sticker == null) return false;
            if (!wallHit.IsValid || wallHit.WallObject == null) return false;
            if (room == null || room.Collider == null) return false;

            var spriteRenderer = sticker.SpriteRenderer;
            if (spriteRenderer == null) return false;

            var bounds = spriteRenderer.bounds;
            var min = bounds.min;
            var max = bounds.max;

            var bottomLeft = new Vector2(min.x, min.y);
            var bottomRight = new Vector2(max.x, min.y);
            var topLeft = new Vector2(min.x, max.y);
            var topRight = new Vector2(max.x, max.y);

            var roomCollider = room.Collider;
            if (!roomCollider.OverlapPoint(bottomLeft)) return false;
            if (!roomCollider.OverlapPoint(bottomRight)) return false;
            if (!roomCollider.OverlapPoint(topLeft)) return false;
            if (!roomCollider.OverlapPoint(topRight)) return false;

            return true;
        }

        private void DestroyStickerInstance()
        {
            if (currentSticker != null)
            {
                Destroy(currentSticker.gameObject);
                currentSticker = null;
            }
        }

        private void ApplyStickerData(Sticker sticker, DragDropData dragData)
        {
            if (dragData?.Data is StickerData stickerData && stickerData.Asset != null)
            {
                var spriteRenderer = sticker.SpriteRenderer;
                if (spriteRenderer != null && stickerData.Sprite != null)
                {
                    spriteRenderer.sprite = stickerData.Sprite;
                    spriteRenderer.sortingLayerName = sortingLayerName;
                }
            }
        }
    }
}
