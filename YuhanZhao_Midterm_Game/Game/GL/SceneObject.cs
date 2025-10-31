using System.Collections.Generic;
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

    public void Destroy(
      IList<PhysicsBody> dynamicBodies,
      IList<PhysicsBody> allBodies,
      IList<SceneObject> objects,
      bool disposeTexture = true)
    {
      objects.Remove(this);
      if (Body != null)
      {
        dynamicBodies.Remove(Body);
        allBodies.Remove(Body);
        Body = null;
      }
      Mesh.Dispose();
      if (disposeTexture && Texture != null)
      {
        Texture.Dispose();
        Texture = null;
      }
    }
  }
}
