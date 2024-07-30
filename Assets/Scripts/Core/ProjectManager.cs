using CircuitSimulation.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace CircuitSimulation.Core
{
    public class ProjectManager : MonoBehaviour
    {
        private static string m_startupProjectName;

        [Header("Debug")]
        [SerializeField]
        private bool m_debugLoadChipStartup;
        [SerializeField]
        private bool m_debugStartupProjectName;
        [SerializeField]
        private bool m_debugChipEditName;

        [Header("References")]
		// private BuiltinChipCodeView m_builtinCodeViewUI;
        [SerializeField]
		private ChipCreator chipEditorPrefab;
        [SerializeField]
		private ChipCreator disableThisOne;
        // [SerializeField]
		// private StarredChipMenuBar m_starredChipMenuBar;
        // [SerializeField]
		// private SimulationController m_simulationController;
        [SerializeField]
		private Transform m_viewChainMenu;
        [SerializeField]
		private TMPro.TMP_Text m_viewChainTextDisplay;
        [SerializeField]
		private Button m_viewChainBackButton;
        // [SerializeField]
		// private CursorGuide m_cursorGuide;

        public event System.Action<ChipCreator> ViewedChipChanged;
        public event System.Action<ChipCreator> EditedChipChanged;
        public event System.Action CurrentChipSaved;
        public event System.Action SavedChipDeleted;

        public ChipCreator ActiveViewChipCreator { get; private set; }
        public ChipCreator ActiveEditChipCreator { get; private set; }
        public ProjectSettings ProjectSettings { get; private set; }

        public void ResaveAll()
        {

        }
    }
}