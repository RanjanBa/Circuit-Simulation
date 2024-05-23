using System;
using System.Collections.Generic;
using CircuitSimulation.Plugins;
using UnityEngine;

namespace CircuitSimulation.Plugins
{
    public class MouseEventSystem : MonoBehaviour
    {
        public enum MouseButton { Left, Middle, Right }

        public event Action onLeftMousePressed;
        public event Action onRightMousePressed;
        public event Action onLeftMouseReleased;
        public event Action onRightMouseReleased;

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
                Debug.LogWarning($"Duplicate Mouse Event System found ({m_instance.gameObject.name}). Deleting this instance ({gameObject.name}).");
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
                vid
            }
        }

        public static void AddInteractionListener(MouseInteractionListener _listener)
        {

        }

    }
}