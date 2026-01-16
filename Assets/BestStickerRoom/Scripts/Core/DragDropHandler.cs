using System;
using UnityEngine;
using UnityEngine.EventSystems;
using BestStickerRoom.Room;
using Zenject;

namespace BestStickerRoom.Core
{
    public class DragDropHandler : MonoBehaviour
    {
        [SerializeField] private float dragThreshold = 10f;

        public event Action<DragDropData> OnDragStarted;
        public event Action<DragDropData> OnDragUpdated;
        public event Action<DragDropData, WallHitResult> OnDragDropped;
        public event Action<DragDropData> OnDragCancelled;

        private InputManager inputManager;
        private IWallDetector wallDetector;
        private Camera raycastCamera;

        private DragDropData currentDragData;
        private bool isDragging;
        private Vector2 dragStartPosition;
        private bool dragThresholdExceeded;

        [Inject]
        private void Construct(InputManager inputMgr, IWallDetector wallDet, [Inject(Id = "RaycastCamera")] Camera camera)
        {
            inputManager = inputMgr;
            wallDetector = wallDet;
            raycastCamera = camera;
        }

        private void OnEnable()
        {
            if (inputManager != null)
            {
                inputManager.OnDragStart += HandleDragStart;
                inputManager.OnDragUpdate += HandleDragUpdate;
                inputManager.OnDragEnd += HandleDragEnd;
            }
        }

        private void OnDisable()
        {
            if (inputManager != null)
            {
                inputManager.OnDragStart -= HandleDragStart;
                inputManager.OnDragUpdate -= HandleDragUpdate;
                inputManager.OnDragEnd -= HandleDragEnd;
            }
        }

        private void HandleDragStart(Vector2 screenPosition)
        {
            if (isDragging) return;

            dragStartPosition = screenPosition;
            dragThresholdExceeded = false;

            var dragSource = FindDragSourceAtPosition(screenPosition);
            if (dragSource != null && dragSource.CanDrag())
            {
                currentDragData = new DragDropData
                {
                    Data = dragSource.GetDragData(),
                    StartScreenPosition = screenPosition,
                    CurrentScreenPosition = screenPosition,
                    SourceGameObject = (dragSource as MonoBehaviour)?.gameObject,
                    Source = dragSource
                };

                isDragging = true;
            }
        }

        private void HandleDragUpdate(Vector2 screenPosition)
        {
            if (!isDragging || currentDragData == null) return;

            var dragDelta = Vector2.Distance(screenPosition, dragStartPosition);
            if (!dragThresholdExceeded && dragDelta > dragThreshold)
            {
                dragThresholdExceeded = true;
                OnDragStarted?.Invoke(currentDragData);
            }

            if (dragThresholdExceeded)
            {
                currentDragData.CurrentScreenPosition = screenPosition;
                OnDragUpdated?.Invoke(currentDragData);
            }
        }

        private void HandleDragEnd(Vector2 screenPosition)
        {
            if (!isDragging || currentDragData == null)
            {
                ResetDragState();
                return;
            }

            if (dragThresholdExceeded)
            {
                var wallHit = wallDetector.DetectWallFromScreenPosition(screenPosition);
                if (wallHit.IsValid)
                {
                    OnDragDropped?.Invoke(currentDragData, wallHit);
                }
                else
                {
                    OnDragCancelled?.Invoke(currentDragData);
                }
            }

            ResetDragState();
        }

        private void ResetDragState()
        {
            isDragging = false;
            currentDragData = null;
            dragThresholdExceeded = false;
        }

        private IDragSource FindDragSourceAtPosition(Vector2 screenPosition)
        {
            if (EventSystem.current == null) return null;

            var pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = screenPosition
            };

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (var result in results)
            {
                if (result.gameObject == null) continue;

                var dragSource = result.gameObject.GetComponent<IDragSource>();
                if (dragSource != null)
                {
                    return dragSource;
                }

                var dragSourceInParent = result.gameObject.GetComponentInParent<IDragSource>();
                if (dragSourceInParent != null)
                {
                    return dragSourceInParent;
                }
            }

            return null;
        }

        public bool IsDragging => isDragging;
        public DragDropData CurrentDragData => currentDragData;
    }
}
