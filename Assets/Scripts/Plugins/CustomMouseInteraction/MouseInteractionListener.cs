using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircuitSimulation.Plugins
{
    public class MouseInteractionListener : MonoBehaviour
    {
        public event System.Action onMouseEntered;
        public event System.Action onMouseExited;
        public event System.Action onLeftMouseDown;
        public event System.Action onRightMouseDown;

        public event System.Action onLeftMouseReleased;
        public event System.Action onRightMouseReleased;

        public event System.Action onLeftClickCompleted;
        public event System.Action onRightClickCompleted;

        private void Awake()
        {
        }       
    }
}