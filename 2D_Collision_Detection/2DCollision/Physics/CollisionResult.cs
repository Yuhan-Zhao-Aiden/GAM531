using OpenTK.Mathematics;

namespace _2DCollision.Physics;

internal readonly struct CollisionResult(bool isColliding, Vector2 normal, float depth)
{
  public bool IsColliding { get; } = isColliding;
  public Vector2 Normal { get; } = normal;
  public float Depth { get; } = depth;
}
