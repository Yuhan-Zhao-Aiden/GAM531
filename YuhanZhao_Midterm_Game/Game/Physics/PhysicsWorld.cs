using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Monolith.Physics
{
  public class PhysicsWorld
  {
    public Vector3 Gravity = new(0f, -9.81f, 0f);
    public List<PhysicsBody> Bodies { get; } = new();

    public const float FixedDt = 1f / 60f;

    // Tweakables
    private const float PenetrationSlop = 0.01f;
    private const float Baumgarte = 0.2f;
    private const float FrictionCoeff = 0.6f;

    public void Step(float dt)
    {
      foreach (var b in Bodies)
      {
        if (b.IsStatic) { b.ClearForce(); continue; }

        Vector3 acceleration = Gravity + (b.Force * b.InverseMass);
        b.Velocity += acceleration * dt;
        b.Position += b.Velocity * dt;

        b.ClearForce();
      }

      var contacts = Collision.FindOverlaps(Bodies);

      const int solverIterations = 8;
      for (int it = 0; it < solverIterations; it++)
      {
        foreach (var c in contacts)
          ResolveContact(c);
      }

      foreach (var c in contacts)
        PositionalCorrection(c);
    }

    private void ResolveContact(Manifold m)
    {
      var A = m.A; var B = m.B;
      if (A.InverseMass + B.InverseMass <= 0f) return;

      Vector3 rv = B.Velocity - A.Velocity;
      float velAlongNormal = Vector3.Dot(rv, m.Normal);
      if (velAlongNormal > 0f) return;

      float e = MathF.Max(A.Restitution, B.Restitution);

      float j = -(1f + e) * velAlongNormal;
      j /= (A.InverseMass + B.InverseMass);

      Vector3 impulse = j * m.Normal;

      if (!A.IsStatic) A.Velocity -= impulse * A.InverseMass;
      if (!B.IsStatic) B.Velocity += impulse * B.InverseMass;

      rv = B.Velocity - A.Velocity;
      Vector3 tangent = rv - Vector3.Dot(rv, m.Normal) * m.Normal;
      float tLenSq = tangent.LengthSquared;
      if (tLenSq > 1e-6f)
      {
        tangent = tangent.Normalized();
        float jt = -Vector3.Dot(rv, tangent);
        jt /= (A.InverseMass + B.InverseMass);

        float mu = FrictionCoeff;
        Vector3 frictionImpulse = Vector3.Zero;
        if (MathF.Abs(jt) < j * mu)
          frictionImpulse = jt * tangent;
        else
          frictionImpulse = -j * mu * tangent;

        if (!A.IsStatic) A.Velocity -= frictionImpulse * A.InverseMass;
        if (!B.IsStatic) B.Velocity += frictionImpulse * B.InverseMass;
      }
    }

    private void PositionalCorrection(Manifold m)
    {
      var A = m.A; var B = m.B;
      float invMassSum = A.InverseMass + B.InverseMass;
      if (invMassSum <= 0f) return;

      float correctionMag = MathF.Max(m.Penetration - PenetrationSlop, 0f) * (Baumgarte / invMassSum);
      Vector3 correction = correctionMag * m.Normal;

      if (!A.IsStatic) A.Position -= correction * A.InverseMass;
      if (!B.IsStatic) B.Position += correction * B.InverseMass;
    }
  }
}
