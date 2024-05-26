using System;
using UnityEngine;
using CircuitSimulation.Plugins;

namespace CircuitSimulation.Core
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Pin : MonoBehaviour
    {
        [SerializeField]
        private float m_interactionRadius;

        [SerializeField]
        private MeshRenderer m_meshRenderer;

        [SerializeField]
        private Color m_defaultColor;

        [SerializeField]
        private Color m_highlightedColor;

        [SerializeField]
        private Color m_highlightedInvalidColor;

        [SerializeField]
        private PinNameDisplay m_nameDisplay;

        private CircleCollider2D m_circleCollider;
        private PinType m_pinType;
        private HighlightState m_activeHighlighState;
        private PinNameDisplayMode m_pinNameDisplayMode;
        private PinDebugInfo m_pinDebugInfo;

        public event Action<Pin> pinDeleted;
        public event Action<Pin> pinMoved;
        public event Action<ThemeColor> themeColorChanged;

        public bool IsBusPin { get; set; }
        public PinState PinState { get; set; }

        public string PinName { get; private set; }
        public int ID { get; private set; }
        public ThemeColor ColorTheme { get; private set; }
        public BaseChip Chip { get; private set; }
        public MouseInteraction<Pin> MouseInteraction { get; private set; }

        public PinType PinType
        {
            get { return m_pinType; }
        }

        public bool IsInputType
        {
            get
            {
                return m_pinType == PinType.ChipInputPin || m_pinType == PinType.SubChipInputPin;
            }
        }

        public bool IsSourcePin
        {
            get { return m_pinType == PinType.ChipInputPin || m_pinType == PinType.ChipOutputPin; }
        }

        public bool IsTargetPin
        {
            get { return !IsSourcePin; }
        }

        public bool IsHighlighted
        {
            get { return m_activeHighlighState != HighlightState.None; }
        }

        public bool IsBelongsToSubChip
        {
            get
            {
                return m_pinType == PinType.SubChipInputPin
                    || m_pinType == PinType.SubChipOutputPin;
            }
        }

        private void Awake()
        {
            m_circleCollider = GetComponent<CircleCollider2D>();
            SetDisplaySize(DisplaySettings.PIN_SIZE);
        }

        private void SetDisplaySize(float _displaySize)
        {
            Transform _parent = transform.parent;
            transform.parent = null;
            transform.localScale = _displaySize * Vector3.one;
            transform.SetParent(_parent);
        }

        private bool ShouldShowPinName()
        {
            switch (m_pinNameDisplayMode)
            {
                case PinNameDisplayMode.Always:
                    return true;
                case PinNameDisplayMode.Hover:
                    return IsHighlighted;
                case PinNameDisplayMode.Toggle:
                    return m_nameDisplay.GetNameVisibility();
                case PinNameDisplayMode.Never:
                    return false;
                default:
                    return false;
            }
        }

        private Color GetHighlightColor(HighlightState _state)
        {
            switch (_state)
            {
                case HighlightState.None:
                    return m_defaultColor;
                case HighlightState.Highlighted:
                    return m_highlightedColor;
                case HighlightState.HighlightedInvalid:
                    return m_highlightedInvalidColor;
                default:
                    return Color.black;
            }
        }

        public void SetUp(
            BaseChip _chip,
            PinDescription _description,
            PinType _pinType,
            ThemeColor _themeColor
        )
        {
            ID = _description.id;

            m_circleCollider.radius = m_interactionRadius;
            m_meshRenderer.material.color = m_defaultColor;
            SetColorTheme(_themeColor, true);

            MouseInteraction = new MouseInteraction<Pin>(gameObject, this);
            m_pinType = _pinType;
            Chip = _chip;

            gameObject.name = $"Pin ({_description.name})";

            m_pinNameDisplayMode = PinNameDisplayMode.Never;
            SetPinName(_description.name);
        }

        public void SetPinName(string _name)
        {
            PinName = _name;
            m_nameDisplay.SetText(name, IsSourcePin);
            UpdateNameDisplayVisibility();
        }

        public void SetColorTheme(ThemeColor _themeColor, bool _isSetUp)
        {
            ColorTheme = _themeColor;
            if (!_isSetUp)
            {
                themeColorChanged?.Invoke(_themeColor);
            }
        }

        public void SetNameDisplayMode(PinNameDisplayMode _displayMode)
        {
            m_pinNameDisplayMode = _displayMode;
            UpdateNameDisplayVisibility();
        }

        public void SetNameVisibility(bool _visibility)
        {
            m_nameDisplay.SetNameVisibility(_visibility);
        }

        public void SetHighlight(HighlightState _state)
        {
            m_activeHighlighState = _state;

            Color _color = GetHighlightColor(_state);

            m_meshRenderer.sharedMaterial.color = _color;
            m_meshRenderer.transform.localScale =
                Vector3.one * (_state == HighlightState.None ? 1f : m_interactionRadius * 2);

            UpdateNameDisplayVisibility();
        }

        public void UpdateNameDisplayVisibility()
        {
            SetNameVisibility(ShouldShowPinName());
        }

        public void NotifyOfDeletion()
        {
            pinDeleted?.Invoke(this);
        }

        public void NotifyMoved()
        {
            pinMoved?.Invoke(this);
        }

        public static int ToInt(PinState state)
        {
            return state == PinState.High ? 1 : 0;
        }
    }
}
