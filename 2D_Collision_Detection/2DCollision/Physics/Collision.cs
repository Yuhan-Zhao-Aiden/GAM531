using System;
using OpenTK.Mathematics;
using _2DCollision.Shapes;

namespace _2DCollision.Physics;

internal static class Collision
{
  public static CollisionResult Resolve(AABB box, Circle circle)
  {
    var closest = box.ClosestPoint(circle.Center);
    var difference = circle.Center - closest;
    var distanceSquared = difference.LengthSquared;
    var radius = circle.Radius;

    if (distanceSquared == 0f)
    {
      var direction = circle.Center - box.Center;
      direction = new Vector2(
          direction.X == 0f ? 0.001f : direction.X,
          direction.Y == 0f ? 0.001f : direction.Y);

      Vector2 normal;
      if (MathF.Abs(direction.X) > MathF.Abs(direction.Y))
      {
        normal = new Vector2(MathF.Sign(direction.X), 0f);
      }
      else
      {
        normal = new Vector2(0f, MathF.Sign(direction.Y));
      }

      if (normal.LengthSquared == 0f)
      {
        normal = Vector2.UnitX;
      }

      return new CollisionResult(true, normal, radius);
    }

    if (distanceSquared > radius * radius)
    {
      return new CollisionResult(false, Vector2.UnitX, 0f);
    }

    var distance = MathF.Sqrt(distanceSquared);
    var normalized = difference / distance;
    var depth = radius - distance;

    return new CollisionResult(true, normalized, depth);
  }
}
