using OpenTK.Mathematics;

namespace _2DCollision.Shapes;

internal readonly struct AABB
{
  public AABB(Vector2 center, Vector2 halfSize)
  {
    Center = center;
    HalfSize = halfSize;
  }

  public Vector2 Center { get; }
  public Vector2 HalfSize { get; }
  public Vector2 Min => Center - HalfSize;
  public Vector2 Max => Center + HalfSize;
  public Vector2 Size => HalfSize * 2f;

  public Vector2 ClosestPoint(Vector2 point)
  {
    var clampedX = MathHelper.Clamp(point.X, Min.X, Max.X);
    var clampedY = MathHelper.Clamp(point.Y, Min.Y, Max.Y);
    return new Vector2(clampedX, clampedY);
  }
}
