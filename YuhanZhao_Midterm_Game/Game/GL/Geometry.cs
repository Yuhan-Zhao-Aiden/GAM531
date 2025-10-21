using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Monolith
{
  public static class Geometry
  {
    public static (float[] vertices, uint[] indices) BuildPlane(float size = 50f, float uvSize = 5f)
    {
      float h = size * 0.5f;
      float[] vertices = new float[]
      {
      -h, 0f, -h,     0f, 1f, 0f,     0f, 0f, // position, normal, uv
      h, 0f, -h,      0f, 1f, 0f,     uvSize, 0f,
      h, 0f, h,     0f, 1f, 0f,     uvSize, uvSize,
      -h, 0f, h,      0f, 1f, 0f,       0f, uvSize
      };

      uint[] indices = new uint[]
      {
      0, 1, 2,
      2, 3, 0
      };

      return (vertices, indices);
    }

    public static (float[] vertices, uint[] indices) BuildCube()
    {
      var verts = new List<float>(24 * 8);
      var idx = new List<uint>(6 * 6);

      void Face(
          Vector3 n,
          Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3 // CCW 
      )
      {
        verts.AddRange(new float[] { v0.X, v0.Y, v0.Z, n.X, n.Y, n.Z, 0f, 0f });
        verts.AddRange(new float[] { v1.X, v1.Y, v1.Z, n.X, n.Y, n.Z, 1f, 0f });
        verts.AddRange(new float[] { v2.X, v2.Y, v2.Z, n.X, n.Y, n.Z, 1f, 1f });
        verts.AddRange(new float[] { v3.X, v3.Y, v3.Z, n.X, n.Y, n.Z, 0f, 1f });

        uint baseV = (uint)verts.Count / 8 - 4;
        idx.Add(baseV + 0); idx.Add(baseV + 1); idx.Add(baseV + 2);
        idx.Add(baseV + 2); idx.Add(baseV + 3); idx.Add(baseV + 0);
      }

      float s = 0.5f; 

      Face(new Vector3(0, 0, 1),
            new Vector3(-s, -s, s), new Vector3(s, -s, s),
            new Vector3(s, s, s), new Vector3(-s, s, s));

      Face(new Vector3(0, 0, -1),
            new Vector3(s, -s, -s), new Vector3(-s, -s, -s),
            new Vector3(-s, s, -s), new Vector3(s, s, -s));

      Face(new Vector3(1, 0, 0),
            new Vector3(s, -s, s), new Vector3(s, -s, -s),
            new Vector3(s, s, -s), new Vector3(s, s, s));

      Face(new Vector3(-1, 0, 0),
            new Vector3(-s, -s, -s), new Vector3(-s, -s, s),
            new Vector3(-s, s, s), new Vector3(-s, s, -s));

      Face(new Vector3(0, 1, 0),
            new Vector3(-s, s, s), new Vector3(s, s, s),
            new Vector3(s, s, -s), new Vector3(-s, s, -s));

      Face(new Vector3(0, -1, 0),
            new Vector3(-s, -s, -s), new Vector3(s, -s, -s),
            new Vector3(s, -s, s), new Vector3(-s, -s, s));

      return (verts.ToArray(), idx.ToArray());
    }
  }

}
