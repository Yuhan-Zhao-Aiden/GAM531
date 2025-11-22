using A9.Core;
using OpenTK.Mathematics;

namespace A9.Graphics;

public class Material
{
  public Shader Shader { get; set; }
  public Texture? Texture { get; set; }
  public Vector3 Color { get; set; } = Vector3.One;
  public float Shininess { get; set; } = 32.0f;

  public Vector3 LightPosition { get; set; }
  public Vector3 LightColor { get; set; } = Vector3.One;
  public Vector3 ViewPosition { get; set; }
  public Matrix4 ViewMatrix { get; set; }
  public Matrix4 ProjectionMatrix { get; set; }

  public Material(Shader shader)
  {
    Shader = shader;
  }

  public Material(Shader shader, Texture texture) : this(shader)
  {
    Texture = texture;
  }

  public void Apply(Transform transform)
  {
    Shader.Use();

    // Set transformation matrices
    Shader.SetMatrix4("model", transform.Model);
    Shader.SetMatrix4("view", ViewMatrix);
    Shader.SetMatrix4("projection", ProjectionMatrix);

    // Set lighting uniforms
    Shader.SetVector3("lightPos", LightPosition);
    Shader.SetVector3("viewPos", ViewPosition);
    Shader.SetVector3("lightColor", LightColor);
    Shader.SetVector3("objectColor", Color);

    if (Texture != null)
    {
      Texture.Bind();
      Shader.SetInt("texture0", 0);
      Shader.SetInt("useTexture", 1);
    }
    else
    {
      Shader.SetInt("useTexture", 0);
    }
  }
}
