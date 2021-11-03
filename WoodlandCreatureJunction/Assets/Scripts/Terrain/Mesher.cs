using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesher
{
    List<Vector3> Points = new List<Vector3>();
    List<int> Triangles = new List<int>();
    List<Color> Colors = new List<Color>();

    /// <summary>
    /// Add a triangle to this mesh. Vertices must be defined in winding order (CCW)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    public void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        Points.Add(a);
        Points.Add(b);
        Points.Add(c);

        Colors.Add(Color.white);
        Colors.Add(Color.white);
        Colors.Add(Color.white);

        Triangles.Add(Points.Count - 1);
        Triangles.Add(Points.Count - 2);
        Triangles.Add(Points.Count - 3);
    }

    /// <summary>
    /// Add a quad to the mesh. Add them in (CCW) order.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    public void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        AddTriangle(a, b, d);
        AddTriangle(b, c, d);
    }

    public void Reset()
    {
        Points.Clear();
        Triangles.Clear();
        Colors.Clear();
    }

    public Mesh GenerateMesh(bool calculateNormals = true)
    {
        Mesh mesh = new Mesh();
        mesh.SetVertices(Points.ToArray());
        mesh.SetColors(Colors.ToArray());
        mesh.SetTriangles(Triangles.ToArray(), 0);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }
}
