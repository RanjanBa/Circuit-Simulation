using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CircuitSimulation.Utilities;

namespace CircuitSimulation.Core
{
    public class ChipInterfaceInteraction : InteractionHandler
    {
        public enum EditorType
        {
            InputEditor,
            OutputEditor
        }

        public enum HandleState
        {
            Default,
            Highlighted,
            Selected
        }

        private const int MAX_GROUP_SIZE = 8;
        private const float FORWARD_DEPTH = -0.1f;

        [SerializeField]
        private EditorType m_editorType;

        [SerializeField]
        private Transform m_chipContainer;

        [SerializeField]
        private ChipSignal m_signalPrefab;

        [SerializeField]
        private RectTransform m_propertiesUI;

        [SerializeField]
        private TMP_InputField m_nameField;

        [SerializeField]
        private Button m_deleteButton;

        [SerializeField]
        private Toggle m_twosComplementToggle;

        [SerializeField]
        private Transform m_signalHolder;

        [Header("Appearence")]
        [SerializeField]
        private Vector2 m_handleSize;

        [SerializeField]
        private Color m_handleColor;

        [SerializeField]
        private Color m_highlightedHandleColor;

        [SerializeField]
        private Color m_selectedHandleColor;

        [SerializeField]
        private float m_propertiesUIX;

        [SerializeField]
        private Vector2 m_propertiesHeightMinMax;

        [SerializeField]
        private bool m_showPreviewSignal;

        [SerializeField]
        private float m_groupSpacing;

        private ChipSignal m_highlightedSignal;
        private ChipSignal[] m_previewSignals;
        private BoxCollider2D m_inputBounds;
        private Mesh m_quadMesh;
        private Material m_handleMaterial;
        private Material m_highlightedHandleMaterial;
        private Material m_selectedHandleMaterial;
        private bool m_isMouseInInputBounds;
        private bool m_isDraggig;
        private float m_dragHandleStartY;
        private float m_dragMouseStartY;
        private int m_currentGroupSize = 1;
        private int m_currentGroupID;
        private Dictionary<int, ChipSignal[]> m_groupsByID;
        private event System.Action<Chip> m_onDeleteChip;
        private event System.Action m_onChipAddedOrDeleted;

        public List<ChipSignal> Signals { get; private set; }
        public List<ChipSignal> SelectedSignals { get; private set; }

        private void Awake() { }

        public override void OrderedUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}