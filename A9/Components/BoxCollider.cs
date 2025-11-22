using A9.Core;
using OpenTK.Mathematics;

namespace A9.Components;

public class BoxCollider : IComponent
{
  public GameObject? GameObject { get; set; }

  public Vector3 Size { get; set; } = Vector3.One;
  public Vector3 Offset { get; set; } = Vector3.Zero;
  public bool IsTrigger { get; set; } = false;
  public string Tag { get; set; } = "Default";

  public BoxCollider() { }

  public BoxCollider(Vector3 size, string tag = "Default")
  {
    Size = size;
    Tag = tag;
  }

  public void Update(float deltaTime)
  {
  }

  public Physics.AABB GetWorldAABB()
  {
    if (GameObject == null)
      return new Physics.AABB(Vector3.Zero, Vector3.Zero);

    Vector3 worldCenter = GameObject.Transform.Position + Offset;
    Vector3 worldSize = Size * GameObject.Transform.Scale;

    return Physics.AABB.FromCenterAndSize(worldCenter, worldSize);
  }

  public bool Intersects(BoxCollider other)
  {
    return GetWorldAABB().Intersects(other.GetWorldAABB());
  }
}
