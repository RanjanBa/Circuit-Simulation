using System.Collections.Generic;
using UnityEngine;

public static class VectorUtility
{
    public static Vector3[] Vector2sToVector3s(IList<Vector2> _vector2s, float _z = 0f)
    {
        Vector3[] _vector3s = new Vector3[_vector2s.Count];
        for (int i = 0; i < _vector3s.Length; i++)
        {
            _vector3s[i] = new Vector3(_vector2s[i].x, _vector2s[i].y, _z);
        }

        return _vector3s;
    }

    public static Vector2[] Vector3sToVector2s(IList<Vector3> _vector3s)
    {
        Vector2[] _vector2s = new Vector2[_vector3s.Count];
        for (int i = 0; i < _vector2s.Length; i++)
        {
            _vector2s[i] = _vector3s[i];
        }

        return _vector2s;
    }
}
