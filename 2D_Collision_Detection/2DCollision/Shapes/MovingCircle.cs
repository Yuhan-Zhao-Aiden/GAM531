using OpenTK.Mathematics;

namespace _2DCollision.Shapes;

internal sealed class MovingCircle
{
  public Vector2 Center;
  public float Radius;
  public Vector2 Velocity;
  public Vector3 BaseColor = Vector3.One;
  public Vector3 CollisionColor = Vector3.One;
  public bool IsColliding;

  public Circle Circle => new(Center, Radius);
  public Vector3 CurrentColor => IsColliding ? CollisionColor : BaseColor;
}
