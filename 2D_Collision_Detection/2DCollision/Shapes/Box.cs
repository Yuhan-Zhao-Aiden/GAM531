using OpenTK.Mathematics;

namespace _2DCollision.Shapes;

internal sealed class Box
{
  public Box(AABB bounds, Vector3 baseColor, Vector3 collisionColor)
  {
    Bounds = bounds;
    BaseColor = baseColor;
    CollisionColor = collisionColor;
  }

  public AABB Bounds { get; }
  public Vector3 BaseColor { get; }
  public Vector3 CollisionColor { get; }
  public bool IsColliding { get; set; }
  public bool IsVisible { get; set; } = true;
  public Vector3 CurrentColor => IsColliding ? CollisionColor : BaseColor;

  public void ToggleVisibility()
  {
    IsVisible = !IsVisible;
  }
}
