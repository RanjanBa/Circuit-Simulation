using System;
using CircuitSimulation.Plugins;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public class WorkArea : MonoBehaviour
    {
        private const float REFERENCE_ORTHO_SIZE = 5f;

        [SerializeField]
        private MeshRenderer m_background;

        [SerializeField]
        private BoxCollider2D m_backgroundCollider;

        [SerializeField]
        private MeshRenderer[] m_outlineEdges;

        [SerializeField]
        private Transform m_inputBar;

        [SerializeField]
        private Transform m_outputBar;

        [SerializeField, Range(0f, 1f)]
        private float m_width;

        [SerializeField, Range(0f, 1f)]
        private float m_height;

        [SerializeField, Range(0f, 1f)]
        private float m_thickness;

        [SerializeField]
        private float m_offsetY;

        [SerializeField]
        private float m_ioBarPadding;

        [SerializeField]
        private Color m_outlineColor;

        [SerializeField]
        private Color m_backgroundColor;

        private Camera m_camera;
        private bool m_isNeedUpdate;

        public event Action workAreaResized;
        public MouseInteraction<WorkArea> workAreaMouseInteraction { get; private set; }
        public MouseInteraction<bool> inputBarMouseInteraction { get; private set; }
        public MouseInteraction<bool> outputBarMouseInteraction { get; private set; }

        public float Width { get; private set; }
        public float Height { get; private set; }

        private void LateUpdate()
        {
            UpdateDisplay();
        }

        private void OnValidate()
        {
            if (Application.isEditor)
            {
                m_isNeedUpdate = true;
            }
        }

        private void UpdateDisplay()
        {
            if (!m_isNeedUpdate)
                return;

            m_isNeedUpdate = false;

            SetWidthAndHeight();

            float _orthoSize = 5;
            float _thickness = m_thickness * _orthoSize * 0.1f;

            float _posY = m_offsetY * _orthoSize / REFERENCE_ORTHO_SIZE;
            transform.position = Vector3.up * _posY;

            m_background.transform.localPosition = new Vector3(0, 0, RenderOrder.BACKGROUND);
            m_background.transform.localScale = new Vector3(
                Width + _thickness / 2,
                Height + _thickness / 2,
                1f
            );
            m_background.sharedMaterial.color = m_backgroundColor;

            m_outlineEdges[0].sharedMaterial.color = m_outlineColor;

            m_outlineEdges[0].transform.localPosition = new Vector3(
                -Width / 2,
                0,
                RenderOrder.BACKGROUND_OUTLINE
            );
            m_outlineEdges[1].transform.localPosition = new Vector3(
                Width / 2,
                0,
                RenderOrder.BACKGROUND_OUTLINE
            );
            m_outlineEdges[2].transform.localPosition = new Vector3(
                0,
                -Height / 2,
                RenderOrder.BACKGROUND_OUTLINE
            );
            m_outlineEdges[3].transform.localPosition = new Vector3(
                0,
                Height / 2,
                RenderOrder.BACKGROUND_OUTLINE
            );
            m_outlineEdges[0].transform.localScale = new Vector3(_thickness, Height, 1);
            m_outlineEdges[1].transform.localScale = new Vector3(_thickness, Height, 1);
            m_outlineEdges[2].transform.localScale = new Vector3(Width + _thickness, _thickness, 1);
            m_outlineEdges[3].transform.localScale = new Vector3(Width + _thickness, _thickness, 1);

            float _ioBarWidth = 1f;
            m_inputBar.localPosition = new Vector3(
                -(Width + _ioBarWidth + m_ioBarPadding) / 2,
                0,
                RenderOrder.BACKGROUND
            );
            m_inputBar.localScale = new Vector3(_ioBarWidth, Height, 1f);

            m_outputBar.localPosition = new Vector3(
                (Width + _ioBarWidth + m_ioBarPadding) / 2,
                0,
                RenderOrder.BACKGROUND
            );
            m_outputBar.localScale = new Vector3(_ioBarWidth, Height, 1f);
        }

        private void SetWidthAndHeight()
        {
            float _screenHeightWorld = 10f;
            float _screenWidthWorld = _screenHeightWorld * 16 / 9;

            Width = _screenWidthWorld * m_width;
            Height = _screenHeightWorld * m_height;

            workAreaResized?.Invoke();
        }

        public void SetUp()
        {
            workAreaMouseInteraction = new MouseInteraction<WorkArea>(
                m_background.gameObject,
                this
            );
            inputBarMouseInteraction = new MouseInteraction<bool>(m_inputBar.gameObject, true);
            outputBarMouseInteraction = new MouseInteraction<bool>(m_outputBar.gameObject, false);

            m_isNeedUpdate = true;
            SetWidthAndHeight();
        }

        public bool IsOutOfBounds(Bounds _bounds)
        {
            Vector2 _min = m_backgroundCollider.bounds.min;
            Vector2 _max = m_backgroundCollider.bounds.max;

            return _bounds.min.x < _min.x
                || _bounds.max.x > _max.x
                || _bounds.min.y < _min.y
                || _bounds.max.y > _max.y;
        }

        public bool IsAnyOutOfBounds(Bounds[] _bounds)
        {
            foreach (Bounds _b in _bounds)
            {
                if (IsOutOfBounds(_b))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
