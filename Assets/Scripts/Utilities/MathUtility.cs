using System.Collections.Generic;
using UnityEngine;

public static class MathUtility
{
    public static Vector2 ClosestPointOnPath(
        Vector2 _point,
        IList<Vector2> _path,
        out int _closestSegmentIndex
    )
    {
        Vector2 _closestPoint = _path[0];
        float _bestDst = float.MaxValue;
        _closestSegmentIndex = 0;

        for (int i = 0; i < _path.Count - 1; i++)
        {
            Vector2 _newClosestPoint = ClosestPointOnLineSegment(_path[i], _path[i + 1], _point);
            float _sqrDst = (_point - _newClosestPoint).sqrMagnitude;

            if (_sqrDst < _bestDst)
            {
                _bestDst = _sqrDst;
                _closestPoint = _newClosestPoint;
                _closestSegmentIndex = i;
            }
        }

        return _closestPoint;
    }

    public static Vector2 ClosestPointOnPath(Vector2 _point, IList<Vector2> _path)
    {
        return ClosestPointOnPath(_point, _path, out _);
    }

    public static Vector2 ClosestPointOnLineSegment(Vector2 _start, Vector2 _end, Vector2 _point)
    {
        Vector2 _dirToEnd = _end - _start;
        Vector2 _dirToPoint = _point - _start;

        float _distOfPath = _dirToEnd.sqrMagnitude;

        if (_distOfPath <= Mathf.Epsilon)
        {
            return _start;
        }

        float _t = Mathf.Clamp01(Vector3.Dot(_dirToPoint, _dirToEnd) / _distOfPath);

        return _start + _t * _dirToPoint;
    }
}
