using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CircuitSimulation.Plugins;
using UnityEngine;

namespace CircuitSimulation.Core
{
    public class Wire : MonoBehaviour
    {
        [SerializeField]
        private WireRenderer m_wireRenderer;

        [SerializeField]
        private EdgeCollider2D m_edgeCollider;

        [SerializeField]
        private float m_wireThickness;

        [SerializeField]
        private float m_wireSelectedThiknessPadding;

        [SerializeField]
        private float m_wireCurveAmount;

        [SerializeField]
        private int m_wireCurveResolution;

        [SerializeField]
        private MeshRenderer m_busConnectionDot;

        private List<Vector2> m_anchorPoints;
        private bool m_isDeleted;

        public event Action<Wire> wireDeleted;

        public MouseInteraction<Wire> MouseInteraction { get; private set; }
        public Pin SourcePin { get; private set; }
        public Pin TargetPin { get; private set; }
        public bool IsConnected { get; private set; }
        public bool IsBusWire { get; private set; }
        public Vector2 CurrentDrawPoint { get; private set; }
        public ThemeColor ColorTheme { get; private set; }
        public ReadOnlyCollection<Vector2> AnchorPoints =>
            new ReadOnlyCollection<Vector2>(m_anchorPoints);

        private void Awake()
        {
            m_wireRenderer.SetThickness(GetThickness(false));
            transform.position = new Vector3(0, 0, RenderOrder.WIRE_EDIT);
            m_edgeCollider.enabled = false;
            MouseInteraction = new MouseInteraction<Wire>(gameObject, this);

            m_anchorPoints = new List<Vector2>();
            SetColor(Color.black);
        }

        private void OnDestroy()
        {
            if (SourcePin != null)
            {
                SourcePin.pinMoved -= OnPinMoved;
            }

            if (TargetPin != null)
            {
                TargetPin.pinMoved -= OnPinMoved;
            }
        }

        private float GetThickness(bool _isSelected)
        {
            return m_wireThickness + (_isSelected ? m_wireSelectedThiknessPadding : 0);
        }

        private void SetColor(Color _color, float _fadeDuration = 0f)
        {
            m_busConnectionDot.sharedMaterial.color = _color;
            m_wireRenderer.SetColor(_color, _fadeDuration);
        }

        private void OnPinMoved(Pin _pin)
        {
            Vector2 _deltaA = (Vector2)SourcePin.transform.position - m_anchorPoints[0];
            Vector2 _deltaB =
                (Vector2)TargetPin.transform.position - m_anchorPoints[m_anchorPoints.Count - 1];

            bool _moveA = _deltaA.magnitude > 0.001f && !SourcePin.IsBusPin;
            bool _moveB = _deltaB.magnitude > 0.001f && !TargetPin.IsBusPin;

            if (_moveA && _moveB)
            {
                for (int i = 0; i < m_anchorPoints.Count; i++)
                {
                    m_anchorPoints[i] += _deltaA;
                }
            }
            else if (_moveA)
            {
                m_anchorPoints[0] += _deltaA;
            }
            else if (_moveB)
            {
                m_anchorPoints[m_anchorPoints.Count - 1] += _deltaB;
            }

            if (_moveA || _moveB)
            {
                UpdateLineRenderer(m_anchorPoints.ToArray());
                UpdateCollider();
            }
        }

        private void UpdateLineRenderer(Vector2[] _points)
        {
            m_wireRenderer.SetAnchorPoints(_points, m_wireCurveAmount, m_wireCurveResolution);
        }

        private void UpdateCollider()
        {
            m_edgeCollider.points = m_anchorPoints.ToArray();
        }

        public void ConnectWireToPins(Pin _pinA, Pin _pinB)
        {
            SourcePin = _pinA.IsSourcePin ? _pinA : _pinB;
            TargetPin = _pinA.IsTargetPin ? _pinA : _pinA;

            IsBusWire = _pinA.IsBusPin && _pinB.IsBusPin;
            IsConnected = true;
            if (SourcePin != _pinA)
            {
                m_anchorPoints.Reverse();
            }

            _pinA.pinDeleted += (_) => DeleteWire();
            _pinB.pinDeleted += (_) => DeleteWire();

            _pinA.pinMoved += OnPinMoved;
            _pinB.pinMoved += OnPinMoved;

            EnableCollision();
            UpdateLineRenderer(m_anchorPoints.ToArray());
            m_wireRenderer.SetThickness(GetThickness(false));
            SourcePin.themeColorChanged += SetColorTheme;
            TargetPin.themeColorChanged += SetColorTheme;

            if ((SourcePin.IsBusPin || TargetPin.IsBusPin) && !IsBusWire)
            {
                m_busConnectionDot.sharedMaterial = new Material(m_busConnectionDot.sharedMaterial);
                Vector2 _busConnectionPoint = SourcePin.IsBusPin
                    ? m_anchorPoints[0]
                    : m_anchorPoints[m_anchorPoints.Count - 1];
                m_busConnectionDot.gameObject.SetActive(true);
                m_busConnectionDot.transform.position = _busConnectionPoint.WithZ(
                    RenderOrder.BUS_CONNECTION_DOT
                );
                m_busConnectionDot.transform.localScale =
                    Vector3.one * DisplaySettings.PIN_SIZE * 0.6f;
            }
        }

        public void DrawToPoint(Vector2 _targetPoint)
        {
            if (m_anchorPoints.Count == 0)
                return;
            if ((m_anchorPoints[m_anchorPoints.Count - 1] - _targetPoint).magnitude > 0.001f)
            {
                CurrentDrawPoint = _targetPoint;
                List<Vector2> _points = new List<Vector2>(m_anchorPoints) { _targetPoint };
                UpdateLineRenderer(_points.ToArray());
            }
        }

        public void EnableCollision()
        {
            UpdateCollider();
            m_edgeCollider.edgeRadius = GetThickness(true);
            m_edgeCollider.enabled = true;
        }

        public void DeleteWire()
        {
            if (m_isDeleted)
                return;

            m_isDeleted = true;
            wireDeleted?.Invoke(this);

            if (SourcePin != null)
            {
                SourcePin.themeColorChanged -= SetColorTheme;
            }

            if (TargetPin != null)
            {
                TargetPin.themeColorChanged -= SetColorTheme;
            }

            Destroy(gameObject);
        }

        public void SetColorTheme(ThemeColor _themeColor)
        {
            ColorTheme = _themeColor;
            UpdateDisplayState();
        }

        public void UpdateDisplayState()
        {
            Color _color = ColorTheme.GetColor(SourcePin.PinState);
            float _z = 0f;
            _z = SourcePin.PinState == PinState.High ? RenderOrder.WIRE_HIGH : RenderOrder.WIRE_LOW;
            if (IsBusWire)
            {
                _z =
                    SourcePin.PinState == PinState.High
                        ? RenderOrder.BUS_WIRE_HIGH
                        : RenderOrder.BUS_WIRE_LOW;
            }

            _z += RenderOrder.LAYER_ABOVE / 10f * ColorTheme.displayPriority;
            transform.position = new Vector3(0, 0, _z);
            SetColor(_color);
        }

        public void SetAnchorPoints(IList<Vector2> _points, bool _updateGraphics)
        {
            m_anchorPoints = new List<Vector2>(_points);
            if (_updateGraphics)
                UpdateLineRenderer(m_anchorPoints.ToArray());
        }

        public void AddAnchorPoint(Vector2 _point)
        {
            if (m_anchorPoints.Count == 0)
            {
                m_anchorPoints.Add(_point);
                return;
            }

            if ((m_anchorPoints[m_anchorPoints.Count - 1] - _point).magnitude > 0.01f)
            {
                m_anchorPoints.Add(_point);
            }
        }

        public void UpdateAnchorPoint(int index, Vector2 _point)
        {
            m_anchorPoints[index] = _point;
            UpdateLineRenderer(m_anchorPoints.ToArray());
        }

        public void RemoveLastAnchorPoint()
        {
            if (m_anchorPoints.Count == 0)
                return;

            m_anchorPoints.RemoveAt(m_anchorPoints.Count - 1);
        }

        public void SetHighlightState(bool _highlighted)
        {
            m_wireRenderer.SetThickness(GetThickness(_highlighted));
        }

        public Vector2 ClosestPoint(Vector2 _point)
        {
            return m_wireRenderer.ClosestPointOnWire(_point);
        }
    }
}
