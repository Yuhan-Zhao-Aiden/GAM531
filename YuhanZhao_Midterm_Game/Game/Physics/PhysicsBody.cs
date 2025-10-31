using OpenTK.Mathematics;

namespace Monolith.Physics
{
  public class PhysicsBody
  {
    public Vector3 Position;
    public Vector3 Velocity;
    public Vector3 HalfExtents;
    public bool IsStatic;
    public float Restitution;

    public Vector3 Force;

    private float _mass = 1f;
    public float InverseMass { get; private set; } = 1f;
    public float Mass
    {
      get => _mass;
      set
      {
        if (value <= 0f ) return;
        _mass = value;
        InverseMass = (IsStatic || float.IsInfinity(value)) ? 0f : 1f / value;
      }
    }

    public Vector3 Min => Position - HalfExtents;
    public Vector3 Max => Position + HalfExtents;

    public PhysicsBody(Vector3 position, Vector3 halfExtents, bool isStatic = false, float restitution = 0.5f)
    {
      Position = position;
      HalfExtents = halfExtents;
      IsStatic = isStatic;
      Restitution = restitution;
      Velocity = Vector3.Zero;
      Force = Vector3.Zero;

      Mass = isStatic ? float.PositiveInfinity : 1f;
    }

    public static PhysicsBody CreateStaticBox(Vector3 position, Vector3 halfExtents)
    {
      return new PhysicsBody(position, halfExtents, isStatic: true, restitution: 0.0f);
    }

    public static PhysicsBody CreateDynamicBox(Vector3 position, Vector3 halfExtents, float restitution = 0.5f)
    {
      return new PhysicsBody(position, halfExtents, isStatic: false, restitution);
    }

    public void ApplyForce(Vector3 exert) { Force += exert; }

    public void ClearForce() { Force = Vector3.Zero; }
  }
}
