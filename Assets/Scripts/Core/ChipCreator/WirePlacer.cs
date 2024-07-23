using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public class WirePlacer : ControllerBase
    {

        private List<Wire> m_allConnectedWires;
        private HashSet<(PinType, PinType)> m_validConnectionsLookup;
        private Pin m_wireStartPin;
        private Wire m_wireStartWire;
        private Wire m_wireUnderConstruction;
        private Wire m_wireUnderMouse;

        private bool m_creatingWireFromPin => IsCreatingWire && m_wireStartPin != null;
        private bool m_creatingWireFromWire => IsCreatingWire && m_wireStartWire != null;

        public bool IsCreatingWire => m_wireUnderConstruction != null;
        public override bool IsBusy() => IsCreatingWire;
        public Wire WireUnderMouse => m_wireUnderMouse;

        public event System.Action<Wire> WireCreated;
        public event System.Action<Wire> WireDeleted;
        public ReadOnlyCollection<Wire> AllWires => new(m_allConnectedWires);

        public override void SetUp(ChipCreator _creator)
        {
            base.SetUp(_creator);

            m_allConnectedWires = new List<Wire>();
            InitValidConnectionLookup();
        }

        private void AddValidConnection(PinType _a, PinType _b)
        {
            m_validConnectionsLookup.Add((_a, _b));
            m_validConnectionsLookup.Add((_b, _a));
        }

        private void InitValidConnectionLookup()
        {
            m_validConnectionsLookup = new HashSet<(PinType, PinType)>();
            AddValidConnection(PinType.ChipInputPin, PinType.ChipOutputPin);
            AddValidConnection(PinType.ChipInputPin, PinType.SubChipInputPin);
            AddValidConnection(PinType.SubChipInputPin, PinType.SubChipOutputPin);
            AddValidConnection(PinType.SubChipOutputPin, PinType.ChipOutputPin);
        }
    }
}
