using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public class ChipDisplay : BaseChip
    {
        [Header("References")]
        [SerializeField]
        private TMP_Text m_nameText;

        [SerializeField]
        private MeshRenderer m_bodyRenderer;

        [SerializeField]
        private MeshRenderer m_bodyOutlineRenderer;

        [SerializeField]
        private MeshRenderer m_highlightRenderer;

        [SerializeField]
        private Pin m_pinPrefab;

        [SerializeField]
        private BoxCollider2D m_interactionBounds;

        [SerializeField]
        private Palette m_palette;

        [Header("Display Settings")]
        [SerializeField]
        private bool m_showChipName = true;

        [SerializeField]
        private Vector2 m_padding;

        [SerializeField]
        private float m_pinSpacingFactor;

        [SerializeField]
        private float m_outlineWidth;

        [SerializeField]
        private float m_outlineDarkenAmount;

        private Color m_outlineColor;

        private string FormatName(string _name)
        {
            _name = System.Text.RegularExpressions.Regex.Replace(_name, @"\s+", " ");

            string[] _words = _name.Split(' ');
            int _maxWordLength = _words.Max(w => w.Length);

            List<string> _lines = new List<string>();
            string _curLine = "";

            for (int i = 0; i < _words.Length; i++)
            {
                if (_curLine.Length + _words[i].Length > _maxWordLength)
                {
                    _lines.Add(_curLine);
                    _curLine = "";
                }

                if (!string.IsNullOrEmpty(_curLine))
                {
                    _curLine += " ";
                }

                _curLine += _words[i];
            }

            _lines.Add(_curLine);

            string _formattedName = _lines[0];

            for (int i = 1; i < _lines.Count; i++)
            {
                _formattedName += "\n" + _lines[i];
            }

            return _formattedName;
        }

        private List<PinDescription> OrderByYPosition(IList<PinDescription> _pinDescriptions)
        {
            List<PinDescription> _sortedPinDescriptions = new List<PinDescription>(
                _pinDescriptions
            );
            _sortedPinDescriptions.Sort((_a, _b) => _b.positionY.CompareTo(_a.positionY));
            return _sortedPinDescriptions;
        }

        private Pin[] InstantiatePins(
            PinDescription[] _pinDescriptions,
            float _posX,
            float _startPosY,
            float _endPosY,
            PinType _pinType
        )
        {
            List<PinDescription> _orderedPins = OrderByYPosition(_pinDescriptions);
            _pinDescriptions = _orderedPins.ToArray();

            Pin[] _pins = new Pin[_pinDescriptions.Length];
            for (int i = 0; i < _pinDescriptions.Length; i++)
            {
                PinDescription _description = _pinDescriptions[i];

                float _t =
                    _pinDescriptions.Length == 1
                        ? 0.5f
                        : (float)_orderedPins.IndexOf(_description) / (_pinDescriptions.Length - 1);
                float _posY = Mathf.Lerp(_startPosY, _endPosY, _t);

                Pin _pin = Instantiate(m_pinPrefab, transform);
                _pin.transform.localPosition = new Vector3(
                    _posX,
                    _posY,
                    RenderOrder.CHIP_PIN - RenderOrder.CHIP
                );
                _pin.SetUp(
                    this,
                    _description,
                    _pinType,
                    m_palette.GetTheme(_description.colorThemeName)
                );
                _pins[i] = _pin;
            }

            return _pins;
        }

        private void Create(ChipDescription _description, Vector2 _position)
        {
            string _displayName = FormatName(NameOfChip);
            m_nameText.text = _displayName;
            m_nameText.fontSize *= _displayName.Length >= 6 ? 0.75f : 1.0f;

            m_nameText.gameObject.SetActive(m_showChipName);
            gameObject.name = $"Chip ({NameOfChip})";

            ColorUtility.TryParseHtmlString(_description.color, out Color _chipColor);
            m_bodyRenderer.material.color = _chipColor;
            m_outlineColor = ColorHelper.Darken(_chipColor, m_outlineDarkenAmount);
            m_bodyOutlineRenderer.material.color = m_outlineColor;
            m_nameText.color = ColorHelper.TextBackOrWhite(_chipColor);

            Vector2 _displaySize = m_showChipName
                ? m_nameText.GetPreferredValues()
                : (Vector2)m_bodyRenderer.transform.localScale;
            float _chipSizeX = _displaySize.x + (m_showChipName ? m_padding.x : 0f);
            int _maxPinsOnOneSide = Mathf.Max(
                _description.inputPinsDescriptions.Length,
                _description.outputPinsDescriptions.Length
            );
            float _pinSpawnLength =
                _maxPinsOnOneSide * DisplaySettings.PIN_SIZE
                + Mathf.Max(0, _maxPinsOnOneSide - 1) * m_pinSpacingFactor;
            float _chipSizeY = Mathf.Max(_pinSpawnLength, _displaySize.y) + m_padding.y;

            Size = new Vector2(_chipSizeX, _chipSizeY);
            m_bodyRenderer.transform.localScale = new Vector3(Size.x, Size.y, 1f);
            m_bodyOutlineRenderer.transform.localScale = new Vector3(
                Size.x + m_outlineWidth,
                Size.y + m_outlineWidth,
                1f
            );
            m_interactionBounds.size = Size + Vector2.one * m_outlineWidth;

            float _pinStartY = (_chipSizeY - m_padding.y - DisplaySettings.PIN_SIZE) / 2;
            float _pinEndY = (-_chipSizeY + m_padding.y + DisplaySettings.PIN_SIZE) / 2;
            Pin[] _inputPins = InstantiatePins(
                _description.inputPinsDescriptions,
                _chipSizeX / 2,
                _pinStartY,
                _pinEndY,
                PinType.SubChipInputPin
            );
            Pin[] _outputPins = InstantiatePins(
                _description.outputPinsDescriptions,
                _chipSizeX / 2,
                _pinStartY,
                _pinEndY,
                PinType.SubChipOutputPin
            );

            SetPins(_inputPins, _outputPins);

            m_highlightRenderer.transform.localScale = (
                new Vector2(_chipSizeX, _chipSizeY)
                + Vector2.one * DisplaySettings.HIGHLIGHT_PADDING
            ).WithZ(1f);
            m_highlightRenderer.gameObject.SetActive(false);

            MouseInteraction = new Plugins.MouseInteraction<BaseChip>(
                m_interactionBounds.gameObject,
                this
            );

            transform.position = _position.WithZ(RenderOrder.CHIP);
        }

        public override ChipInfo GetChipInfo()
        {
            return new ChipInfo()
            {
                name = NameOfChip,
                id = ID,
                points = new Point[] { new Point(transform.position.x, transform.position.y) }
            };
        }

        public override void Load(ChipDescription _description, ChipInfo _chipInfo)
        {
            base.Load(_description, _chipInfo);
            Create(_description, Point.ToVector(_chipInfo.points[0]));
        }

        public override void StartPlacing(ChipDescription _description, int _id)
        {
            base.StartPlacing(_description, _id);
            Create(_description, Vector2.zero);
        }

        public override void SetHighlightState(bool _isHighlighted)
        {
            m_highlightRenderer.gameObject.SetActive(_isHighlighted);
            m_bodyOutlineRenderer.sharedMaterial.color = _isHighlighted
                ? Color.black
                : m_outlineColor;
        }

        public override Bounds GetBounds()
        {
            return m_interactionBounds.bounds;
        }

        public override void NotifyMoved()
        {
            foreach (Pin _pin in AllPins)
            {
                _pin.NotifyMoved();
            }
        }
    }
}
