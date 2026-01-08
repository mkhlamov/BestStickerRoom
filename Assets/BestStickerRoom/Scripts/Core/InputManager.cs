using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BestStickerRoom.Core
{
    public class InputManager : MonoBehaviour, XRIDefaultInputActions.IXRIUIActions, XRIDefaultInputActions.ITouchscreenGesturesActions
    {
        [SerializeField] private bool enableOnStart = true;

        private XRIDefaultInputActions inputActions;
        private Vector2 lastPointPosition;

        public event Action<Vector2> OnTap;
        public event Action<Vector2> OnClick;
        public event Action<Vector2> OnPoint;
        public event Action<Vector2> OnDragStart;
        public event Action<Vector2> OnDragUpdate;
        public event Action<Vector2> OnDragEnd;

        private void Awake()
        {
            inputActions = new XRIDefaultInputActions();
        }

        private void OnEnable()
        {
            if (inputActions != null)
            {
                inputActions.XRIUI.SetCallbacks(this);
                inputActions.TouchscreenGestures.SetCallbacks(this);
                inputActions.Enable();
            }
        }

        private void OnDisable()
        {
            if (inputActions != null)
            {
                inputActions.XRIUI.RemoveCallbacks(this);
                inputActions.TouchscreenGestures.RemoveCallbacks(this);
                inputActions.Disable();
            }
        }

        private void Start()
        {
            if (enableOnStart)
            {
                inputActions?.Enable();
            }
        }

        private void OnDestroy()
        {
            inputActions?.Dispose();
        }

        void XRIDefaultInputActions.IXRIUIActions.OnPoint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var position = context.ReadValue<Vector2>();
                lastPointPosition = position;
                OnPoint?.Invoke(position);
            }
        }

        void XRIDefaultInputActions.IXRIUIActions.OnClick(InputAction.CallbackContext context)
        {
            if (context.performed && !IsMouseButtonPressed())
            {
                var position = GetCurrentPointerPosition();
                OnClick?.Invoke(position);
            }
        }

        private bool IsMouseButtonPressed()
        {
            var mouse = Mouse.current;
            return mouse != null && mouse.leftButton.isPressed;
        }

        void XRIDefaultInputActions.ITouchscreenGesturesActions.OnTapStartPosition(InputAction.CallbackContext context) { }

        void XRIDefaultInputActions.IXRIUIActions.OnNavigate(InputAction.CallbackContext context) { }
        void XRIDefaultInputActions.IXRIUIActions.OnSubmit(InputAction.CallbackContext context) { }
        void XRIDefaultInputActions.IXRIUIActions.OnCancel(InputAction.CallbackContext context) { }
        void XRIDefaultInputActions.IXRIUIActions.OnScrollWheel(InputAction.CallbackContext context) { }
        void XRIDefaultInputActions.IXRIUIActions.OnMiddleClick(InputAction.CallbackContext context) { }
        void XRIDefaultInputActions.IXRIUIActions.OnRightClick(InputAction.CallbackContext context) { }

        void XRIDefaultInputActions.ITouchscreenGesturesActions.OnDragStartPosition(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var position = context.ReadValue<Vector2>();
                OnDragStart?.Invoke(position);
            }
        }

        void XRIDefaultInputActions.ITouchscreenGesturesActions.OnDragCurrentPosition(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var position = context.ReadValue<Vector2>();
                lastPointPosition = position;
                OnDragUpdate?.Invoke(position);
            }
        }

        void XRIDefaultInputActions.ITouchscreenGesturesActions.OnDragDelta(InputAction.CallbackContext context) { }
        void XRIDefaultInputActions.ITouchscreenGesturesActions.OnPinchStartPosition(InputAction.CallbackContext context) { }
        void XRIDefaultInputActions.ITouchscreenGesturesActions.OnPinchGap(InputAction.CallbackContext context) { }
        void XRIDefaultInputActions.ITouchscreenGesturesActions.OnPinchGapDelta(InputAction.CallbackContext context) { }
        void XRIDefaultInputActions.ITouchscreenGesturesActions.OnTwistStartPosition(InputAction.CallbackContext context) { }
        void XRIDefaultInputActions.ITouchscreenGesturesActions.OnTwistDeltaRotation(InputAction.CallbackContext context) { }
        void XRIDefaultInputActions.ITouchscreenGesturesActions.OnScreenTouchCount(InputAction.CallbackContext context) { }
        void XRIDefaultInputActions.ITouchscreenGesturesActions.OnSpawnObject(InputAction.CallbackContext context) { }

        private bool isMouseDragging;
        private Vector2 lastMousePosition;

        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            var currentPosition = mouse.position.ReadValue();

            if (mouse.leftButton.wasPressedThisFrame)
            {
                isMouseDragging = true;
                lastMousePosition = currentPosition;
                OnDragStart?.Invoke(currentPosition);
            }
            else if (mouse.leftButton.isPressed && isMouseDragging)
            {
                if (Vector2.Distance(currentPosition, lastMousePosition) > 0.1f)
                {
                    lastMousePosition = currentPosition;
                    OnDragUpdate?.Invoke(currentPosition);
                }
            }
            else if (mouse.leftButton.wasReleasedThisFrame && isMouseDragging)
            {
                isMouseDragging = false;
                OnDragEnd?.Invoke(currentPosition);
            }
        }

        private Vector2 GetCurrentPointerPosition()
        {
            var pointer = Pointer.current;
            if (pointer != null)
            {
                return pointer.position.ReadValue();
            }

            var mouse = Mouse.current;
            if (mouse != null)
            {
                return mouse.position.ReadValue();
            }

            return lastPointPosition;
        }
    }
}

