using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace CircuitSimulation.Plugins
{
    public class MouseEventSystem : MonoBehaviour
    {
        public enum MouseButton
        {
            Left,
            Middle,
            Right
        }

        public event Action leftMousePressed;
        public event Action rightMousePressed;
        public event Action leftMouseReleased;
        public event Action lightMouseReleased;

        private Camera m_mainCamera;
        private List<MouseInteractionListener> m_listenersWithMouseOver;
        private List<MouseInteractionListener> m_listenersWithoutMouseOver;

        private HashSet<MouseInteractionListener> m_listenersWithLeftMouseDown;
        private HashSet<MouseInteractionListener> m_listenersWithRightMouseDown;

        private Transform m_lastHit;
        private Vector2 m_mouseSmoothPos;
        private Vector2 m_mouseSmoothVel;
        private float m_mouseSmoothTime;

        private static MouseEventSystem m_instance;

        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
                Initialize();
            }
            else
            {
                Debug.LogWarning(
                    $"Duplicate Mouse Event System found ({m_instance.gameObject.name}). Deleting this instance ({gameObject.name})."
                );
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            m_listenersWithMouseOver = new List<MouseInteractionListener>();
            m_listenersWithoutMouseOver = new List<MouseInteractionListener>();
            m_listenersWithLeftMouseDown = new HashSet<MouseInteractionListener>();
            m_listenersWithRightMouseDown = new HashSet<MouseInteractionListener>();

            m_lastHit = null;
            m_mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Application.isEditor)
            {
                m_mouseSmoothPos = Vector2.SmoothDamp(
                    m_mouseSmoothPos,
                    Mouse.current.position.ReadValue(),
                    ref m_mouseSmoothVel,
                    m_mouseSmoothTime
                );
            }

            GameObject _hitGO = GetObjectUnderMouse();

            if (_hitGO)
            {
                NotifyMouseExit(m_lastHit, _hitGO.transform);
                NotifyMouseEnter(_hitGO.transform);
                m_lastHit = _hitGO.transform;
            }

            HandleMouseButtonEvents();
        }

        private void OnDestroy()
        {
            if (m_instance == null)
                return;

            m_instance = null;
        }

        private void HandleMouseButtonEvents()
        {
            Mouse _mouse = Mouse.current;

            if (_mouse.leftButton.wasPressedThisFrame)
            {
                leftMousePressed?.Invoke();

                foreach (var _lis in m_listenersWithMouseOver)
                {
                    _lis.OnMousePressedDown(MouseButton.Left);
                    m_listenersWithLeftMouseDown.Add(_lis);
                }
            }

            if (_mouse.rightButton.wasPressedThisFrame)
            {
                rightMousePressed?.Invoke();
                foreach (var _lis in m_listenersWithMouseOver)
                {
                    m_listenersWithRightMouseDown.Add(_lis);
                }
            }

            if (_mouse.leftButton.wasReleasedThisFrame)
            {
                leftMouseReleased?.Invoke();
                foreach (var _lis in m_listenersWithMouseOver)
                {
                    _lis.OnMouseReleased(MouseButton.Left);
                    if (m_listenersWithLeftMouseDown.Contains(_lis))
                    {
                        _lis.OnClickCompleted(MouseButton.Left);
                    }
                }

                m_listenersWithLeftMouseDown.Clear();
            }

            if (_mouse.rightButton.wasReleasedThisFrame)
            {
                rightMousePressed?.Invoke();
                foreach (var _lis in m_listenersWithMouseOver)
                {
                    _lis.OnMouseReleased(MouseButton.Right);
                    if (m_listenersWithRightMouseDown.Contains(_lis))
                    {
                        _lis.OnClickCompleted(MouseButton.Right);
                    }
                }

                m_listenersWithRightMouseDown.Clear();
            }
        }

        private void NotifyMouseEnter(Transform _hit)
        {
            if (_hit == null)
                return;

            for (int i = m_listenersWithoutMouseOver.Count - 1; i >= 0; i--)
            {
                MouseInteractionListener _lis = m_listenersWithoutMouseOver[i];

                if (_hit.IsChildOf(_lis.transform))
                {
                    _lis.OnMouseEntered();
                    m_listenersWithMouseOver.Add(_lis);
                    m_listenersWithoutMouseOver.RemoveAt(i);
                }
            }
        }

        private void NotifyMouseExit(Transform _lastHit, Transform _newHit)
        {
            for (int i = m_listenersWithMouseOver.Count - 1; i >= 0; i--)
            {
                MouseInteractionListener _lis = m_listenersWithMouseOver[i];

                if (_newHit == null || !_newHit.IsChildOf(_lis.transform))
                {
                    _lis.OnMouseExited();
                    m_listenersWithoutMouseOver.Add(_lis);
                    m_listenersWithMouseOver.RemoveAt(i);
                }
            }
        }

        private GameObject GetObjectUnderMouse()
        {
            Mouse _mouse = Mouse.current;

            InputSystemUIInputModule _uiInputModule =
                EventSystem.current?.currentInputModule as InputSystemUIInputModule;

            if (_uiInputModule)
            {
                RaycastResult _lastRaycastResult = _uiInputModule.GetLastRaycastResult(
                    _mouse.deviceId
                );
                return _lastRaycastResult.gameObject;
            }

            Vector2 _mouseScreenPos = m_mouseSmoothPos;
            Vector2 _mouseWorldPos = m_mainCamera.ScreenToWorldPoint(_mouseScreenPos);
            Collider2D _col = Physics2D.OverlapPoint(_mouseWorldPos);
            if (_col)
            {
                return _col.gameObject;
            }
            return null;
        }

        private void RegisterMouseInteractionListener(MouseInteractionListener _listener)
        {
            if (m_lastHit != null && m_lastHit.IsChildOf(_listener.transform))
            {
                _listener.OnMouseEntered();
                m_listenersWithMouseOver.Add(_listener);
            }
            else
            {
                m_listenersWithoutMouseOver.Add(_listener);
            }
        }

        private void DeRegisterMouseInteractionListener(MouseInteractionListener _listener)
        {
            if (!m_listenersWithMouseOver.Remove(_listener))
            {
                m_listenersWithoutMouseOver.Remove(_listener);
            }

            m_listenersWithLeftMouseDown.Remove(_listener);
            m_listenersWithRightMouseDown.Remove(_listener);
        }

        public void SetMouseSmoothingTime(float _time)
        {
            m_mouseSmoothTime = _time;
        }

        public static Vector2 GetMousePos()
        {
            if (m_instance)
                return Vector2.zero;

            return m_instance.m_mouseSmoothPos;
        }

        public static void AddMouseInteractionListener(MouseInteractionListener _listener)
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<MouseEventSystem>();

                if (m_instance == null)
                {
                    GameObject _gm = new GameObject("Mouse Event System");
                    m_instance = _gm.AddComponent<MouseEventSystem>();
                }

                m_instance.Initialize();
            }

            m_instance.RegisterMouseInteractionListener(_listener);
        }

        public static void RemoveMouseInteractionListener(MouseInteractionListener _listener)
        {
            if (m_instance == null)
                return;

            m_instance.DeRegisterMouseInteractionListener(_listener);
        }
    }
}
