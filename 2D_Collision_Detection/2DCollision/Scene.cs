using System;
using OpenTK.Mathematics;
using _2DCollision.Physics;
using _2DCollision.Rendering;
using _2DCollision.Shapes;

namespace _2DCollision;

internal sealed class Scene
{
  private readonly Box[] _boxes;
  private readonly MovingCircle _circle;
  private Vector2 _playArea;

  private Scene(Box[] boxes, MovingCircle circle, Vector2 playArea)
  {
    _boxes = boxes;
    _circle = circle;
    _playArea = playArea;
  }

  public static Scene CreateDefault(float width, float height)
  {
    var boxSize = new Vector2(60f, 200f);
    var padding = 90f;

    var boxes = new[]
    {
            new Box(
                new AABB(new Vector2(padding, height * 0.5f), boxSize * 0.5f),
                new Vector3(0.2f, 0.7f, 0.9f),
                new Vector3(0.9f, 0.5f, 0.2f)),
            new Box(
                new AABB(new Vector2(width - padding, height * 0.5f), boxSize * 0.5f),
                new Vector3(0.2f, 0.9f, 0.4f),
                new Vector3(0.9f, 0.5f, 0.2f))
        };

    var circle = new MovingCircle
    {
      Center = new Vector2(width * 0.5f, height * 0.5f),
      Radius = 35f,
      Velocity = new Vector2(220f, 0f),
      BaseColor = new Vector3(0.95f, 0.95f, 0.95f),
      CollisionColor = new Vector3(0.95f, 0.25f, 0.35f)
    };

    return new Scene(boxes, circle, new Vector2(width, height));
  }

  public void Update(float deltaTime)
  {
    _circle.Center += _circle.Velocity * deltaTime;
    _circle.IsColliding = false;

    foreach (var box in _boxes)
    {
      box.IsColliding = false;
    }

    foreach (var box in _boxes)
    {
      var result = Collision.Resolve(box.Bounds, _circle.Circle);

      if (!result.IsColliding)
      {
        continue;
      }

      box.IsColliding = true;
      _circle.IsColliding = true;
      _circle.Center += result.Normal * result.Depth;
      _circle.Velocity = VectorMath.Reflect(_circle.Velocity, result.Normal);
      box.ToggleVisibility();
      break;
    }

    KeepCircleInVerticalBounds();
  }

  public void Render(ShapeRenderer renderer, Matrix4 projection)
  {
    foreach (var box in _boxes)
    {
      if (box.IsVisible)
      {
        renderer.DrawRectangle(
            projection,
            box.Bounds.Center,
            box.Bounds.Size,
            box.CurrentColor);
      }
    }

    renderer.DrawCircle(projection, _circle.Center, _circle.Radius, _circle.CurrentColor);
  }

  public void OnViewportChanged(float width, float height)
  {
    _playArea = new Vector2(width, height);
  }

  private void KeepCircleInVerticalBounds()
  {
    const float padding = 25f;
    var minY = padding + _circle.Radius;
    var maxY = _playArea.Y - padding - _circle.Radius;

    if (_circle.Center.Y < minY)
    {
      _circle.Center.Y = minY;
      _circle.Velocity.Y = MathF.Abs(_circle.Velocity.Y);
    }
    else if (_circle.Center.Y > maxY)
    {
      _circle.Center.Y = maxY;
      _circle.Velocity.Y = -MathF.Abs(_circle.Velocity.Y);
    }
  }
}
