using UnityEngine;

class Quad
{
    private Vector3[] shape = new Vector3[4];

    private static readonly int[] tris = new int[6]
    {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
    };
    private static readonly Vector3[] normals = new Vector3[4]
    {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
    };
    private static readonly Vector2[] uv = new Vector2[4]
    {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
    };

    public Mesh ToMesh(Mesh mesh)
    {
        mesh.vertices = shape;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.uv = uv;
        return mesh;
    }

    public void SetQuad(float x1, float y1, float w1, float x2, float y2, float w2)
    {
        shape[0].x = (x1 - w1);
        shape[0].y = y1;
        shape[1].x = (x1 + w1);
        shape[1].y = y1;
        shape[2].x = (x2 - w2);
        shape[2].y = y2;
        shape[3].x = (x2 + w2);
        shape[3].y = y2;
    }
}

