using OpenTK.Mathematics;

namespace _2DCollision.Shapes;

internal readonly struct Circle(Vector2 center, float radius)
{
  public Vector2 Center { get; } = center;
  public float Radius { get; } = radius;
}
