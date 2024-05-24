using System;
using UnityEngine;

namespace CircuitSimulation.Plugins
{
    public class MouseInteraction<T>
    {
        public event Action<T> mouseEntered;
        public event Action<T> mouseExited;
        public event Action<T> leftMouseDown;
        public event Action<T> rightMouseDown;
        public event Action<T> leftMouseReleased;
        public event Action<T> rightMouseReleased;
        public event Action<T> leftClickCompleted;
        public event Action<T> rightClickCompleted;

        public bool IsMouseOverObject { get; private set; }

        public MouseInteraction(GameObject _listenerTarget, T _eventContext)
        {
            MouseInteractionListener _lis =
                _listenerTarget.GetComponent<MouseInteractionListener>();

            _lis.mouseEntered += () =>
            {
                IsMouseOverObject = true;
                mouseEntered?.Invoke(_eventContext);
            };

            _lis.mouseExited += () =>
            {
                IsMouseOverObject = false;
                mouseExited?.Invoke(_eventContext);
            };

            _lis.leftMouseDown += () =>
            {
                leftMouseDown?.Invoke(_eventContext);
            };

            _lis.rightMouseDown += () =>
            {
                rightMouseDown?.Invoke(_eventContext);
            };

            _lis.leftMouseReleased += () =>
            {
                leftMouseReleased?.Invoke(_eventContext);
            };

            _lis.rightMouseReleased += () =>
            {
                rightMouseReleased?.Invoke(_eventContext);
            };

            _lis.leftClickCompleted += () =>
            {
                leftClickCompleted?.Invoke(_eventContext);
            };

            _lis.rightClickCompleted += () =>
            {
                rightClickCompleted?.Invoke(_eventContext);
            };
        }
    }
}
