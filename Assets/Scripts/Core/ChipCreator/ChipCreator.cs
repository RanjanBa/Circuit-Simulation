using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CircuitSimulation.Plugins;
using CircuitSimulation.Utilities;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public class ChipCreator : MonoBehaviour
    {
        [SerializeField]
        private WorkArea m_workArea;
        [SerializeField]
        private Palette m_palette;

        private List<BaseChip> m_allSubChips;
        private Dictionary<int, BaseChip> m_subChipById;
        private Dictionary<int, Pin> m_mainPinById;
        private ControllerBase[] m_controllers;
        private bool m_subChipPinNamesVisible;
        private bool m_mainChipPinNamesVisible;

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
        public ChipDescription LastSavedDescription { get; private set; }

        public Wire WireUnderMouse => WirePlacer.WireUnderMouse;
        public ProjectSettings settings;
        public string ProjectName => settings.projectName;
        public Palette ColorThemes => m_palette;

        public ReadOnlyCollection<BaseChip> allSubChips => new ReadOnlyCollection<BaseChip>(m_allSubChips);

        public void SetUp(ProjectSettings _settings, bool _isViewOnly)
        {
            ChipCreatorActions = new ChipCreatorActions();
            ChipCreatorActions.Enable();

            CanEdit = !_isViewOnly;
            settings = _settings;
            // PinInteractions = new();

            m_controllers = GetComponentsInChildren<ControllerBase>();
            ChipPlacer = m_controllers.OfType<ChipPlacer>().First();
            PinPlacer = m_controllers.OfType<PinPlacer>().First();
            WirePlacer = m_controllers.OfType<WirePlacer>().First();
            ChipSelector = m_controllers.OfType<ChipSelector>().First();
            ChipMover = m_controllers.OfType<ChipMover>().First();

            m_allSubChips = new List<BaseChip>();
            m_subChipById = new Dictionary<int, BaseChip>();
            m_mainPinById = new Dictionary<int, Pin>();
        }
    }
}
