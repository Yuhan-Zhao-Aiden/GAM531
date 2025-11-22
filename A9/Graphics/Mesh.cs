using OpenTK.Graphics.OpenGL4;

namespace A9.Graphics;

public class Mesh : IDisposable
{
  private int _vao;
  private int _vbo;
  private int _ebo;
  private int _indexCount;
  private bool _disposed = false;

  public Mesh(float[] vertices, uint[] indices)
  {
    _indexCount = indices.Length;

    // Generate VAO, VBO, EBO
    _vao = GL.GenVertexArray();
    _vbo = GL.GenBuffer();
    _ebo = GL.GenBuffer();

    GL.BindVertexArray(_vao);

    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
    GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

    int stride = 8 * sizeof(float); // position(3) + normal(3) + UV(2)

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
    GL.BindVertexArray(_vao);
    GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
    GL.BindVertexArray(0);
  }

  public void Dispose()
  {
    if (!_disposed)
    {
      GL.DeleteBuffer(_vbo);
      GL.DeleteBuffer(_ebo);
      GL.DeleteVertexArray(_vao);
      _disposed = true;
    }
    GC.SuppressFinalize(this);
  }
}
