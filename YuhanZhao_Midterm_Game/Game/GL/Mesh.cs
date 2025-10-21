using OpenTK.Graphics.OpenGL;

namespace Monolith
{
  public enum MeshType { Plane, Cube };
  public class Mesh: IDisposable
  {
    private MeshType Type { get; }
    // private float[] vertices;
    // private uint[] indices;
    private int vao, vbo, ebo;
    private int indexCount;

    public Mesh(MeshType type, float[] vertices, uint[] indices, int stride = 8 * sizeof(float))
    {
      this.Type = type;
      this.indexCount = indices.Length;
      vao = GL.GenVertexArray();
      vbo = GL.GenBuffer();
      ebo = GL.GenBuffer();

      GL.BindVertexArray(vao);
      GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
      GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

      GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
      GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
      GL.EnableVertexAttribArray(0);

      GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
      GL.EnableVertexAttribArray(1);

      GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
      GL.EnableVertexAttribArray(2);

      GL.BindVertexArray(0);
    }

    public void Draw()
    {
      GL.BindVertexArray(vao);
      GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
    }

    public void Dispose()
    {
      GL.DeleteVertexArray(vao);
      GL.DeleteBuffer(vbo);
      GL.DeleteBuffer(ebo);
    }
  }
}