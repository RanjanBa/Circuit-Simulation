using System;
using CircuitSimulation.Plugins;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CircuitSimulation.Core
{
    public class EditablePinHandle : MonoBehaviour
    {
        [SerializeField]
        private EditablePin m_editablePin;

        [SerializeField]
        private Color m_normalColor;

        [SerializeField]
        private Color m_highlightedColor;

        [SerializeField]
        private Color m_selectedColor;

        [SerializeField]
        private MeshRenderer m_meshRenderer;

        private bool m_isDragging;
        private Material m_material;
        private bool m_isSelected;
        private Vector2 m_dragMousePos;
        private Vector2 m_dragStartPos;

        public event Action<EditablePin> handleSelected;
        public event Action<EditablePin> handleDeselected;
        public event Action<EditablePin> handleMoved;
        public MouseInteraction<EditablePinHandle> MouseInteraction { get; private set; }

        public bool IsDragging
        {
            get { return m_isDragging; }
        }

        private void LateUpdate()
        {
            if (MouseHelper.IsLeftMouseReleasedThisFrame() && m_isDragging)
            {
                m_isDragging = false;
                float _z =
                    m_editablePin.PinState == PinState.High
                        ? RenderOrder.EDITABLE_PIN_HIGH
                        : RenderOrder.EDITABLE_PIN;
                m_editablePin.transform.position = m_editablePin.transform.position.WithZ(_z);
            }

            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                OnDeselected();
            }

            if (m_isDragging)
            {
                float _mouseY = MouseHelper.GetMouseWorldPosition().y;
                float _posY = m_dragStartPos.y + (_mouseY - m_dragMousePos.y);

                if (Mathf.Abs(_posY - m_editablePin.transform.position.y) > 0.0001f)
                {
                    m_editablePin.transform.position = new Vector3(
                        m_dragStartPos.x,
                        _posY,
                        RenderOrder.EDITABLE_PIN_PREVIEW
                    );
                    handleMoved?.Invoke(m_editablePin);
                    m_editablePin.GetPin().NotifyMoved();
                }
            }
        }

        private void OnMouseEntered(EditablePinHandle _handle)
        {
            if (m_isSelected)
                return;

            SetColor(m_highlightedColor);
        }

        private void OnMouseExited(EditablePinHandle _handle)
        {
            if (m_isSelected)
                return;

            SetColor(m_normalColor);
        }

        private void OnSelect()
        {
            m_isDragging = true;
            m_dragMousePos = MouseHelper.GetMouseWorldPosition();
            m_dragStartPos = m_editablePin.transform.position;

            if (!m_isSelected)
            {
                m_isSelected = true;
                SetColor(m_selectedColor);
                handleSelected?.Invoke(m_editablePin);
            }
        }

        private void OnDeselected()
        {
            if (!m_isSelected)
                return;

            m_isSelected = false;
            SetColor(m_normalColor);
            handleDeselected?.Invoke(m_editablePin);
        }

        public void SetUp()
        {
            MouseInteraction = new MouseInteraction<EditablePinHandle>(gameObject, this);

            MouseInteraction.mouseEntered += OnMouseEntered;
            MouseInteraction.mouseExited += OnMouseExited;

            m_material = Instantiate(m_meshRenderer.sharedMaterial);
            m_meshRenderer.sharedMaterial = m_material;
            SetColor(m_normalColor);
        }

        public void Select()
        {
            OnSelect();
        }

        public void Deselect()
        {
            OnDeselected();
        }

        public void SetColor(Color _color)
        {
            m_material.color = _color;
        }
    }
}
