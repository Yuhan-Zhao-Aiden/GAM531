using System;
using OpenTK.Mathematics;

namespace _2DCollision.Rendering;

internal sealed class ShapeRenderer : IDisposable
{
  private readonly ShaderProgram _shader;
  private readonly Mesh _quad;
  private readonly Mesh _circle;
  private bool _disposed;

  public ShapeRenderer()
  {
    _shader = new ShaderProgram(VertexSource, FragmentSource);
    _quad = Mesh.CreateQuad();
    _circle = Mesh.CreateCircle(48);
  }

  public void DrawRectangle(Matrix4 projection, Vector2 center, Vector2 size, Vector3 color)
      => DrawMesh(_quad, projection, BuildModel(center, size), color);

  public void DrawCircle(Matrix4 projection, Vector2 center, float radius, Vector3 color)
  {
    var size = new Vector2(radius * 2f);
    DrawMesh(_circle, projection, BuildModel(center, size), color);
  }

  private void DrawMesh(Mesh mesh, Matrix4 projection, Matrix4 model, Vector3 color)
  {
    _shader.Use();
    _shader.SetMatrix4("uProjection", projection);
    _shader.SetMatrix4("uModel", model);
    _shader.SetVector3("uColor", color);
    mesh.Draw();
  }

  private static Matrix4 BuildModel(Vector2 center, Vector2 size)
  {
    var scale = Matrix4.CreateScale(size.X, size.Y, 1f);
    var translate = Matrix4.CreateTranslation(center.X, center.Y, 0f);
    return scale * translate;
  }

  public void Dispose()
  {
    if (_disposed)
    {
      return;
    }

    _disposed = true;
    _shader.Dispose();
    _quad.Dispose();
    _circle.Dispose();
  }

  private const string VertexSource = @"
        #version 330 core
        layout(location = 0) in vec2 aPosition;

        uniform mat4 uProjection;
        uniform mat4 uModel;

        void main()
        {
            gl_Position = uProjection * uModel * vec4(aPosition, 0.0, 1.0);
        }";

  private const string FragmentSource = @"
        #version 330 core
        out vec4 FragColor;
        uniform vec3 uColor;

        void main()
        {
            FragColor = vec4(uColor, 1.0);
        }";
}
