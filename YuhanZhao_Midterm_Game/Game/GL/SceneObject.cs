using OpenTK.Mathematics;
using Monolith.Physics;

namespace Monolith.core
{

  public class SceneObject
  {
    public Mesh Mesh { get; }
    public Texture? Texture { get; set; }
    public Transform Transform { get; }
    public PhysicsBody? Body { get; set; }

    public SceneObject(Mesh mesh, Texture? texture, Transform? transform = null, PhysicsBody? body = null)
    {
      Mesh = mesh;
      Texture = texture;
      Transform = transform ?? new Transform();
      Body = body;
    }
  }
}
