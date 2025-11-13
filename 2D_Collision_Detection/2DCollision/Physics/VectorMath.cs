using OpenTK.Mathematics;

namespace _2DCollision.Physics;

internal static class VectorMath
{
  public static Vector2 Reflect(Vector2 vector, Vector2 normal)
  {
    if (normal.LengthSquared == 0f)
    {
      return vector;
    }

    var normalized = normal.Normalized();
    var projection = 2f * Vector2.Dot(vector, normalized);
    return vector - projection * normalized;
  }
}
