using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CircuitSimulation.Plugins;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public class ChipCreator : MonoBehaviour
    {
        public event Action<BaseChip> subChipAdded;
        public event Action<BaseChip> subChipDeleted;
        public ReadOnlyCollection<MouseInteraction<Pin>> PinInteractions { get; private set; }
        public ChipCreatorActions ChipCreatorActions { get; private set; }

        public bool CanEdit { get; private set; }
        public ChipPlacer ChipPlacer { get; private set; }
        public ChipSelector ChipSelector { get; private set; }
        public ChipMover ChipMover { get; private set; }
        public PinPlacer PinPlacer { get; private set; }
        public WirePlacer WirePlacer { get; private set; }
        public WorkArea WorkArea { get; private set; }
        public BaseChip ChipUnderMouse { get; private set; }
        public Pin PinUnderMouse { get; private set; }
        public Wire WireUnderMouse { get; private set; }

        public ChipDescription LastSavedDescription { get; private set; }

        public Wire WireUnderMouse => WirePlacer.WireUnderMouse;
    }
}
