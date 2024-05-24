using System;
using System.Collections.Generic;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public class Pin : MonoBehaviour
    {
        public enum HighlightState
        {
            None,
            Highlighted,
            HighlightedInvalid
        }

        public event Action<Pin> pinDeleted;
        public event Action<Pin> pinMoved;

        public int ID { get; private set; }

        public void NotifyOfDeletion()
        {
            pinDeleted?.Invoke(this);
        }
    }
}
