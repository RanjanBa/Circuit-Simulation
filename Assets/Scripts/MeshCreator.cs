using UnityEngine;

public static class MeshCreator
{
    public static void CreateQuadMesh(ref Mesh _mesh)
    {
        if (_mesh == null)
        {
            _mesh = new Mesh();
        }
        _mesh.Clear();

        Vector3[] _vertices =
        {
            (Vector3.left + Vector3.up) * 0.5f,
            (Vector3.right + Vector3.up) * 0.5f,
            (Vector3.left + Vector3.down) * 0.5f,
            (Vector3.right + Vector3.down) * 0.5f
        };

        int[] _tries = { 0, 1, 2, 1, 3, 2 };

        _mesh.vertices = _vertices;
        _mesh.triangles = _tries;
        _mesh.RecalculateBounds();
    }
}
