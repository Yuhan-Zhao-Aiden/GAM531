using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace _2DCollision.Rendering;

internal sealed class Mesh : IDisposable
{
  private readonly int vao;
  private readonly int vbo;
  private readonly PrimitiveType _primitiveType;
  private readonly int _vertexCount;
  private bool _disposed;

  private Mesh(ReadOnlySpan<float> vertices, PrimitiveType primitiveType)
  {
    _primitiveType = primitiveType;
    _vertexCount = vertices.Length / 2;

    vao = GL.GenVertexArray();
    vbo = GL.GenBuffer();

    GL.BindVertexArray(vao);
    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);
    GL.EnableVertexAttribArray(0);
    GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

    GL.BindVertexArray(0);
  }

  public static Mesh CreateQuad()
  {
    ReadOnlySpan<float> vertices = stackalloc float[]
    {
            -0.5f, -0.5f,
             0.5f, -0.5f,
             0.5f,  0.5f,
            -0.5f, -0.5f,
             0.5f,  0.5f,
            -0.5f,  0.5f
        };

    return new Mesh(vertices, PrimitiveType.Triangles);
  }

  public static Mesh CreateCircle(int segments)
  {
    var vertices = new float[(segments + 2) * 2];
    vertices[0] = 0f;
    vertices[1] = 0f;

    for (int i = 0; i <= segments; i++)
    {
      var angle = MathHelper.TwoPi * i / segments;
      var x = MathF.Cos(angle) * 0.5f;
      var y = MathF.Sin(angle) * 0.5f;
      var index = (i + 1) * 2;
      vertices[index] = x;
      vertices[index + 1] = y;
    }

    return new Mesh(vertices, PrimitiveType.TriangleFan);
  }

  public void Draw()
  {
    GL.BindVertexArray(vao);
    GL.DrawArrays(_primitiveType, 0, _vertexCount);
    GL.BindVertexArray(0);
  }

  public void Dispose()
  {
    if (_disposed)
    {
      return;
    }

    _disposed = true;
    GL.DeleteVertexArray(vao);
    GL.DeleteBuffer(vbo);
  }
}
