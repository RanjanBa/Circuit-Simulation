using System;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public class EditablePin : MonoBehaviour
    {
        [SerializeField]
        private Pin m_pin;

        [SerializeField]
        private EditablePinIndicator m_indicatorPin;

        [SerializeField]
        private EditablePinHandle m_handlePin;

        [SerializeField]
        private Transform[] m_flips;

        private bool m_isInputPin;

        public event Action<EditablePin> editablePinDeleted;

        public string PinName
        {
            get { return m_pin.PinName; }
        }

        public PinState PinState
        {
            get { return m_pin.PinState; }
        }

        public Pin GetPin() => m_pin;

        public EditablePinHandle GetEditablePinHandle() => m_handlePin;

        private void TogglePinState()
        {
            if (m_pin.PinState == PinState.Low)
            {
                m_pin.PinState = PinState.High;
            }
            else if (m_pin.PinState == PinState.High)
            {
                m_pin.PinState = PinState.Low;
            }
        }

        public void SetUp(bool _isInputPin, string _name, ThemeColor _themeColor, int _id)
        {
            m_isInputPin = _isInputPin;
            ConfigureGraphics(_isInputPin);

            PinDescription _description = new PinDescription()
            {
                name = _name,
                id = _id,
                positionY = transform.position.y
            };

            m_pin.SetUp(
                null,
                _description,
                _isInputPin ? PinType.ChipInputPin : PinType.ChipOutputPin,
                _themeColor
            );
            m_pin.themeColorChanged += (_) => UpdateDisplayColor();

            UpdateDisplayColor();

            if (_isInputPin)
            {
                m_indicatorPin.mouseInteraction.leftMouseDown += (_) => TogglePinState();
            }

            m_pin.PinState = PinState.Low;
            m_handlePin.SetUp();
        }

        public void SetName(string _name)
        {
            m_pin.SetPinName(_name);
        }

        public void UpdateDisplayState()
        {
            transform.position = transform.position.WithZ(
                PinState == PinState.High ? RenderOrder.EDITABLE_PIN_HIGH : RenderOrder.EDITABLE_PIN
            );
            UpdateDisplayColor();
        }

        public void UpdateDisplayColor()
        {
            m_indicatorPin.SetColor(m_pin.ColorTheme.GetColor(PinState));
        }

        public void Delete()
        {
            m_pin.NotifyOfDeletion();
            editablePinDeleted?.Invoke(this);
            Destroy(gameObject);
        }

        public void ConfigureGraphics(bool _isInput)
        {
            if (!_isInput)
            {
                foreach (Transform _trans in m_flips)
                {
                    _trans.localPosition = new Vector3(
                        -_trans.localPosition.x,
                        -_trans.localPosition.y,
                        _trans.localPosition.z
                    );
                }
            }
        }
    }
}
