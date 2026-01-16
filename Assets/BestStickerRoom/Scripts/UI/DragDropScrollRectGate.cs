using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BestStickerRoom.Core;
using UniRx;
using Zenject;

namespace BestStickerRoom.UI
{
    public class DragDropScrollRectGate : IDragDropGate, IInitializable, IDisposable
    {
        public ReactiveProperty<bool> DragAllowed { get; } = new(false);
        
        private readonly InputManager inputManager;

        private const float DRAG_AXIS_THRESHOLD = 10f;
        private Vector2 dragStartPosition;
        private ScrollRect activeScrollRect;

        public DragDropScrollRectGate(InputManager inputMgr)
        {
            inputManager = inputMgr;
        }

        public void Initialize()
        {
            if (inputManager == null) return;

            inputManager.OnDragStart += HandleDragStart;
            inputManager.OnDragUpdate += HandleDragUpdate;
            inputManager.OnDragEnd += HandleDragEnd;
        }

        public void Dispose()
        {
            if (inputManager == null) return;

            inputManager.OnDragStart -= HandleDragStart;
            inputManager.OnDragUpdate -= HandleDragUpdate;
            inputManager.OnDragEnd -= HandleDragEnd;

            RestoreScrollRect();
            ResetState();
        }

        private void HandleDragStart(Vector2 screenPosition)
        {
            dragStartPosition = screenPosition;
            DragAllowed.Value = false;

            activeScrollRect = FindScrollRectAtPosition(screenPosition);
        }

        private void HandleDragUpdate(Vector2 screenPosition)
        {
            var delta = screenPosition - dragStartPosition;
            var absDelta = new Vector2(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
            
            if (activeScrollRect == null)
            {
                if (absDelta.magnitude >= DRAG_AXIS_THRESHOLD)
                {
                    DragAllowed.Value = true;
                }

                return;
            }
            
            if (DragAllowed.Value || !activeScrollRect.enabled) return;

            if (absDelta.x >= DRAG_AXIS_THRESHOLD && absDelta.x >= absDelta.y)
            {
                DragAllowed.Value = false;
                return;
            }

            if (absDelta.y >= DRAG_AXIS_THRESHOLD && absDelta.y > absDelta.x)
            {
                DragAllowed.Value = true;
                DisableScrollRect();
            }
        }

        private void HandleDragEnd(Vector2 screenPosition)
        {
            RestoreScrollRect();

            ResetState();
        }

        private void ResetState()
        {
            DragAllowed.Value = false;
            activeScrollRect = null;
        }

        private ScrollRect FindScrollRectAtPosition(Vector2 screenPosition)
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

                var scrollRect = result.gameObject.GetComponentInParent<ScrollRect>();
                if (scrollRect != null && scrollRect.enabled)
                {
                    return scrollRect;
                }
            }

            return null;
        }

        private void DisableScrollRect()
        {
            if (activeScrollRect == null) return;
            activeScrollRect.enabled = false;
        }

        private void RestoreScrollRect()
        {
            if (activeScrollRect == null) return;
            activeScrollRect.enabled = true;
            activeScrollRect = null;
        }
    }
}
