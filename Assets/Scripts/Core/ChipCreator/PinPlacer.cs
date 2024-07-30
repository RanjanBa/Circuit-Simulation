using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public class PinPlacer : ControllerBase
    {
        private enum PinPreviewState { None, PreviewInput, PreviewOutput }

        [SerializeField]
        private Transform m_ioPinHolder;
        [SerializeField]
        private EditablePin m_editablePinPrefab;
        [SerializeField]
        private Color m_pinPreviewColor;

        private List<EditablePin> m_inputPins;
        private List<EditablePin> m_outputPins;
        private Transform m_previewInputPin;
        private Transform m_previewOutputPin;
        private PinPreviewState m_pinPreviewState;
        private EditablePin m_selectedPin;
        private System.Random m_random;

        public event System.Action<EditablePin> PinCreated;
        public event System.Action<EditablePin> PinDeleted;
        public ReadOnlyCollection<EditablePin> InputPins { get; private set; }
        public ReadOnlyCollection<EditablePin> OutputPins { get; private set; }
        public ReadOnlyCollection<EditablePin> AllPins { get; private set; }

        private void Update()
        {
            if (m_chipCreator.AnyControllerBusy() || pinPreviewState == PinPreviewState.None)
            {
                previewInputPin.gameObject.SetActive(false);
                previewOutputPin.gameObject.SetActive(false);
            }
            else if (pinPreviewState == PinPreviewState.PreviewInput)
            {
                previewInputPin.gameObject.SetActive(true);
                previewInputPin.position = GetPosition(true).WithZ(RenderOrder.EditablePinPreview);
            }
            else if (pinPreviewState == PinPreviewState.PreviewOutput)
            {
                previewOutputPin.gameObject.SetActive(true);
                previewOutputPin.position = GetPosition(false).WithZ(RenderOrder.EditablePinPreview);
            }
        }


        public override void SetUp(ChipCreator _creator)
        {
            base.SetUp(_creator);

            m_random = new System.Random();

            m_inputPins = new List<EditablePin>();
            m_outputPins = new List<EditablePin>();

            _creator.WorkArea.inputBarMouseInteraction.leftMouseDown += (input) => AddPin(true, true);
            _creator.WorkArea.outputBarMouseInteraction.leftMouseDown += (input) => AddPin(false, true);

            _creator.WorkArea.inputBarMouseInteraction.mouseEntered += OnMouseEnterPinBar;
            _creator.WorkArea.outputBarMouseInteraction.mouseEntered += OnMouseEnterPinBar;
            _creator.WorkArea.inputBarMouseInteraction.mouseExitted += OnMouseExitBar;
            _creator.WorkArea.outputBarMouseInteraction.mouseExitted += OnMouseExitBar;

            _creator.WorkArea.workAreaResized += OnWorkAreaResized;

            m_previewInputPin = CreatePreviewPin(true);
            m_previewOutputPin = CreatePreviewPin(false);
        }
    }
}
