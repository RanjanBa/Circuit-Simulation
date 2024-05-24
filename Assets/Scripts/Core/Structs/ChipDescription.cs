using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public struct ChipDescription
    {
        public string name;
        public string color;
        public PinDescription[] _inputPinsDescriptions;
        public PinDescription[] _outputPinsDescriptions;
        public ChipInfo[] subChips;
    }
}
