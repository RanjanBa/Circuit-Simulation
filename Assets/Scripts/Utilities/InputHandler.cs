using UnityEngine;
using UnityEngine.EventSystems;

namespace CircuitSimulation.Utilities
{
    public static class InputHandler
    {
        private static Camera m_mainCamera;

        public static Camera MainCamera
        {
            get
            {
                if (m_mainCamera == null)
                {
                    m_mainCamera = Camera.main;
                }
                return m_mainCamera;
            }
        }

        public static Vector2 MousePosToWorldPos
        {
            get { return MainCamera.ScreenToWorldPoint(Input.mousePosition); }
        }

        public static bool IsMouseOverUI
        {
            get { return EventSystem.current.IsPointerOverGameObject(); }
        }

        public static GameObject GetObjectOnMousePos(LayerMask _mask)
        {
            Vector2 _mouseWorldPos = MousePosToWorldPos;
            Ray _ray = new Ray(new Vector3(_mouseWorldPos.x, _mouseWorldPos.y, -100), Vector3.forward);
            RaycastHit2D _hit = Physics2D.GetRayIntersection(_ray, float.MaxValue, _mask);

            if (_hit.collider)
            {
                return _hit.collider.gameObject;
            }

            return null;
        }

        public static bool AnyOfGivenKeysDown(params KeyCode[] _keys)
        {
            foreach (var _key in _keys)
            {
                if (Input.GetKeyDown(_key))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
