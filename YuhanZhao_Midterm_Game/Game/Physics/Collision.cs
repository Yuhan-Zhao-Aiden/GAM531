// https://developer.mozilla.org/en-US/docs/Games/Techniques/3D_collision_detection
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Monolith.Physics
{
  public static class Collision
  {
    public static bool AabbOverlap(PhysicsBody a, PhysicsBody b)
    {
      if (a.Max.X < b.Min.X || a.Min.X > b.Max.X) return false;
      if (a.Max.Y < b.Min.Y || a.Min.Y > b.Max.Y) return false;
      if (a.Max.Z < b.Min.Z || a.Min.Z > b.Max.Z) return false;// If no overlap on any axis = no collision
      return true;
    }

    public static Manifold BuildManifold(PhysicsBody a, PhysicsBody b)
    {
      float dx1 = b.Max.X - a.Min.X;
      float dx2 = a.Max.X - b.Min.X;
      float dy1 = b.Max.Y - a.Min.Y;
      float dy2 = a.Max.Y - b.Min.Y;
      float dz1 = b.Max.Z - a.Min.Z;
      float dz2 = a.Max.Z - b.Min.Z;

      float overlapX = System.MathF.Min(dx1, dx2);
      float overlapY = System.MathF.Min(dy1, dy2);
      float overlapZ = System.MathF.Min(dz1, dz2);

      // Pick smallest axis of penetration
      var m = new Manifold { A = a, B = b, Penetration = 0f, Normal = Vector3.Zero };
      if (overlapX <= 0f || overlapY <= 0f || overlapZ <= 0f) return m;

      // Determine normal sign by centers
      Vector3 centerDelta = b.Position - a.Position;

      if (overlapX <= overlapY && overlapX <= overlapZ)
        m = new Manifold { A = a, B = b, Penetration = overlapX, Normal = new Vector3(System.MathF.Sign(centerDelta.X), 0, 0) };
      else if (overlapY <= overlapX && overlapY <= overlapZ)
        m = new Manifold { A = a, B = b, Penetration = overlapY, Normal = new Vector3(0, System.MathF.Sign(centerDelta.Y), 0) };
      else
        m = new Manifold { A = a, B = b, Penetration = overlapZ, Normal = new Vector3(0, 0, System.MathF.Sign(centerDelta.Z)) };

      return m;
    }

    public static List<Manifold> FindOverlaps(IReadOnlyList<PhysicsBody> bodies)
    {
      var contacts = new List<Manifold>();
      for (int i = 0; i < bodies.Count; i++)
      {
        for (int j = i + 1; j < bodies.Count; j++)
        {
          var a = bodies[i]; var b = bodies[j];
          if (a.IsStatic && b.IsStatic) continue;
          if (!AabbOverlap(a, b)) continue;

          var m = BuildManifold(a, b);
          if (m.IsValid) contacts.Add(m);
        }
      }
      return contacts;
    }
  }
}