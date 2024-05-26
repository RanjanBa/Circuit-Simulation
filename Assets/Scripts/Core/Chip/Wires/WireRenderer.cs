using System.Collections.Generic;
using UnityEngine;

namespace CircuitSimulation.Core
{
    [RequireComponent(typeof(LineRenderer))]
    public class WireRenderer : MonoBehaviour
    {
        private LineRenderer m_lineRenderer;
        private List<Vector2> m_drawPoints;
        private Material m_material;
        private bool m_isInitialized;
        private bool m_animatingColor;
        private Color m_previousColor;
        private Color m_targetColor;
        private float m_colorAnimateDuration;
        private float m_colorAnimateTime;

        private void Initialize()
        {
            if (m_isInitialized)
                return;

            m_isInitialized = true;
            m_lineRenderer = GetComponent<LineRenderer>();
            m_drawPoints = new List<Vector2>();

            m_material = Instantiate(m_lineRenderer.sharedMaterial);
            m_lineRenderer.sharedMaterial = m_material;
        }

        private void Update()
        {
            if (!m_animatingColor)
                return;

            m_colorAnimateTime += Time.deltaTime / m_colorAnimateDuration;
            m_material.color = Color.Lerp(m_previousColor, m_targetColor, m_colorAnimateTime);

            if (m_colorAnimateTime >= 1)
            {
                m_animatingColor = false;
            }
        }

        public void SetThickness(float _width)
        {
            Initialize();
            m_lineRenderer.startWidth = _width;
            m_lineRenderer.endWidth = _width;
        }

        public void SetColor(Color _color, float _fadeDuration = 0f)
        {
            Initialize();
            m_previousColor = m_material.color;
            if (_fadeDuration > 0f)
            {
                m_animatingColor = true;
                m_targetColor = _color;
                m_colorAnimateDuration = _fadeDuration;
                m_colorAnimateTime = 0;
            }
            else
            {
                m_animatingColor = false;
                m_material.color = _color;
            }
        }

        public void SetAnchorPoints(
            Vector2[] _anchorPoints,
            float _curveSize,
            int _resolution,
            bool _useWorldSpace = false
        )
        {
            Initialize();
            m_drawPoints.Clear();
            m_drawPoints.Add(_anchorPoints[0]);

            for (int i = 1; i < _anchorPoints.Length - 1; i++)
            {
                Vector2 _targetPoint = _anchorPoints[i];
                Vector2 _targetDir = (_anchorPoints[i] - _anchorPoints[i - 1]).normalized;
                float _dstToTarget = (_anchorPoints[i] - _anchorPoints[i - 1]).magnitude;
                float _dstToCurveStart = Mathf.Max(_dstToTarget - _curveSize, _dstToTarget / 2);

                Vector2 _nextTargetDir = (_anchorPoints[i + 1] - _anchorPoints[i]).normalized;
                float _dstToNextTarget = (_anchorPoints[i + 1] - _anchorPoints[i]).magnitude;

                Vector2 _curveStartPoint = _anchorPoints[i - 1] + _targetDir * _dstToCurveStart;
                Vector2 _curveEndPoint =
                    _targetPoint + _nextTargetDir * Mathf.Min(_curveSize, _dstToNextTarget / 2);

                for (int j = 0; j < _resolution; j++)
                {
                    float _t = j / (_resolution - 1f);
                    Vector2 _a = Vector2.Lerp(_curveStartPoint, _targetPoint, _t);
                    Vector2 _b = Vector2.Lerp(_targetPoint, _curveEndPoint, _t);
                    Vector2 _p = Vector2.Lerp(_a, _b, _t);

                    if ((_p - m_drawPoints[m_drawPoints.Count - 1]).sqrMagnitude > 0.001f)
                    {
                        m_drawPoints.Add(_p);
                    }
                }

                m_drawPoints.Add(_anchorPoints[_anchorPoints.Length - 1]);
                m_lineRenderer.positionCount = m_drawPoints.Count;

                m_lineRenderer.SetPositions(VectorUtility.Vector2sToVector3s(m_drawPoints));
                m_lineRenderer.useWorldSpace = _useWorldSpace;
            }
        }

        public Vector2 ClosestPointOnWire(Vector2 _point)
        {
            return MathUtility.ClosestPointOnPath(_point, m_drawPoints);
        }
    }
}
