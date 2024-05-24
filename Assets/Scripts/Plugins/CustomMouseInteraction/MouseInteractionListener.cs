using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircuitSimulation.Plugins
{
    public class MouseInteractionListener : MonoBehaviour
    {
        public event System.Action mouseEntered;
        public event System.Action mouseExited;
        public event System.Action leftMouseDown;
        public event System.Action rightMouseDown;
        public event System.Action leftMouseReleased;
        public event System.Action rightMouseReleased;
        public event System.Action leftClickCompleted;
        public event System.Action rightClickCompleted;

        private void Awake()
        {
            MouseEventSystem.AddMouseInteractionListener(this);
        }

        private void OnDestroy()
        {
            MouseEventSystem.RemoveMouseInteractionListener(this);
        }

        public void OnMouseEntered()
        {
            mouseEntered?.Invoke();
        }

        public void OnMouseExited()
        {
            mouseExited?.Invoke();
        }

        public void OnMousePressedDown(MouseEventSystem.MouseButton _mouseBtn)
        {
            if (_mouseBtn == MouseEventSystem.MouseButton.Left)
            {
                leftMouseDown?.Invoke();
            }
            else if (_mouseBtn == MouseEventSystem.MouseButton.Right)
            {
                rightMouseDown?.Invoke();
            }
        }

        public void OnMouseReleased(MouseEventSystem.MouseButton _mouseBtn)
        {
            if (_mouseBtn == MouseEventSystem.MouseButton.Left)
            {
                leftMouseReleased?.Invoke();
            }
            else if (_mouseBtn == MouseEventSystem.MouseButton.Right)
            {
                rightMouseReleased?.Invoke();
            }
        }

        public void OnClickCompleted(MouseEventSystem.MouseButton _mouseBtn)
        {
            if (_mouseBtn == MouseEventSystem.MouseButton.Left)
            {
                leftClickCompleted?.Invoke();
            }
            else if (_mouseBtn == MouseEventSystem.MouseButton.Right)
            {
                rightClickCompleted?.Invoke();
            }
        }
    }
}
