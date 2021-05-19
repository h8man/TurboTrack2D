using UnityEngine;

class Quad
{
    private Vector3[] shape;
    private int offset;
    private Vector3[] normals;
    private Color[] color;
    private int[] tris;
    private Vector2[] uv;

    public Quad(int capacity = 1000)
    {
        shape = new Vector3[capacity];
        normals = new Vector3[capacity];
        tris = new int[capacity*4];
        uv = new Vector2[capacity];
        color = new Color[capacity];

        for (int i = 0; i < capacity / 4; i+=4)
        {
            normals[i] = -Vector3.forward;
            normals[i + 1] = -Vector3.forward;
            normals[i + 2] = -Vector3.forward;
            normals[i + 3] = -Vector3.forward;
            
            uv[i] = new Vector2(0, 0);
            uv[i + 1] = new Vector2(1, 0);
            uv[i + 2] = new Vector2(0, 1);
            uv[i + 3] = new Vector2(1, 1);
            
            color[i] = new Color(1,0,0);
            color[i + 1] = new Color(0, 1, 0);
            color[i + 2] = new Color(0, 0, 0);
            color[i + 3] = new Color(1, 0, 0);

        }
        
        for (int i = 0,j = 0; i < capacity; i += 6, j+=4)
        {
            tris[i] = 0+j;
            tris[i + 1] = 2+j;
            tris[i + 2] = 1+j;
            tris[i + 3] = 2+j;
            tris[i + 4] = 3+j;
            tris[i + 5] = 1+j;
        }
    }

    //private static readonly int[] tris = new int[6]
    //{
    //        // lower left triangle
    //        0, 2, 1,
    //        // upper right triangle
    //        2, 3, 1
    //};
    //private static readonly Vector3[] normals = new Vector3[4]
    //{
    //        -Vector3.forward,
    //        -Vector3.forward,
    //        -Vector3.forward,
    //        -Vector3.forward
    //};
    //private static readonly Vector2[] uv = new Vector2[4]
    //{
    //        new Vector2(0, 0),
    //        new Vector2(1, 0),
    //        new Vector2(0, 1),
    //        new Vector2(1, 1)
    //};

    public Mesh ToMesh(Mesh mesh)
    {
        mesh.SetVertices(shape, 0, offset);
        mesh.SetTriangles(tris, 0, offset/4*6, 0, true, 0);
        mesh.SetNormals(normals, 0, offset);
        mesh.SetUVs(0, uv, 0, offset);
        mesh.SetColors(color, 0, offset);
        return mesh;
    }

    public void Clear() { offset = 0; }

    public void SetQuad(float x1, float y1, float w1, float x2, float y2, float w2, float z)
    {
        var i = offset;
        shape[offset].x = (x1 - w1);
        shape[offset++].y = y1;
        shape[offset].x = (x1 + w1);
        shape[offset++].y = y1;
        shape[offset].x = (x2 - w2);
        shape[offset++].y = y2;
        shape[offset].x = (x2 + w2);
        shape[offset++].y = y2;

        color[i].b = z;
        color[i + 1].b = z;
        color[i + 2].b = z;
        color[i + 3].b = z;
    }
}

