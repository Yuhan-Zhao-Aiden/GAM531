using A9.Core;
using OpenTK.Mathematics;

namespace A9.Components;

public class RigidBody : IComponent
{
  public GameObject? GameObject { get; set; }

  public Vector3 Velocity { get; set; } = Vector3.Zero;
  public bool UseGravity { get; set; } = true;
  public bool IsKinematic { get; set; } = false;
  public float Gravity { get; set; } = -20.0f;
  public float Drag { get; set; } = 0.1f;

  public RigidBody() { }

  public RigidBody(bool useGravity, bool isKinematic = false)
  {
    UseGravity = useGravity;
    IsKinematic = isKinematic;
  }

  public void Update(float deltaTime)
  {
  }

  public void AddForce(Vector3 force)
  {
    if (!IsKinematic)
    {
      Velocity += force;
    }
  }
}
