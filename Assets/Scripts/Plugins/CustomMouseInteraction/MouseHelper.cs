using UnityEngine;
using UnityEngine.InputSystem;

namespace CircuitSimulation.Plugins
{
    public static class MouseHelper
    {
        private static Camera m_camera;

        public static Camera Camera
        {
            get
            {
                if (m_camera == null)
                {
                    m_camera = Camera.main;
                }

                return m_camera;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            m_camera = null;
        }

        public static bool IsLeftMousePressedThisFrame()
        {
            return Mouse.current.leftButton.wasPressedThisFrame;
        }

        public static bool IsLeftMousePressed()
        {
            return Mouse.current.leftButton.isPressed;
        }

        public static bool IsLeftMouseReleasedThisFrame()
        {
            return Mouse.current.leftButton.wasReleasedThisFrame;
        }

        public static bool IsRightMousePressedThisFrame()
        {
            return Mouse.current.rightButton.wasPressedThisFrame;
        }

        public static Vector2 GetMouseScreenPosition()
        {
            if (Application.isEditor)
            {
                return MouseEventSystem.GetMousePos();
            }

            return Mouse.current.position.ReadValue();
        }

        public static Vector2 GetMouseWorldPosition()
        {
            return Camera.ScreenToWorldPoint(GetMouseScreenPosition());
        }

        public static Vector3 GetMouseWorldPosition(float _z)
        {
            Vector2 _pos = GetMouseWorldPosition();

            return new Vector3(_pos.x, _pos.y, _z);
        }

        public static Vector2 CalculateAxisSnappedMousePosition(Vector2 _origin, bool _snap = true)
        {
            Vector2 _snappedMousePos = GetMouseWorldPosition();

            if (_snap)
            {
                Vector2 _delta = _snappedMousePos - _origin;
                bool _snappedHorizontal = Mathf.Abs(_delta.x) > Mathf.Abs(_delta.y);
                _snappedMousePos = new Vector2(
                    _snappedHorizontal ? _snappedMousePos.x : _origin.x,
                    _snappedHorizontal ? _origin.y : _snappedMousePos.y
                );
            }

            return _snappedMousePos;
        }
    }
}
