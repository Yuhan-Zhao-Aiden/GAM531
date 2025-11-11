using System;
using OpenTK.Mathematics;

namespace knight;

public abstract class SceneObject : IDisposable
{
  protected SceneObject(Vector2 position, SpriteRenderer spriteRenderer)
  {
    Position = position;
    SpriteRenderer = spriteRenderer ?? throw new ArgumentNullException(nameof(spriteRenderer));
  }

  public Vector2 Position { get; set; }
  public Vector2 Velocity { get; set; }
  public SpriteRenderer SpriteRenderer { get; }

  public virtual Vector2 Size => SpriteRenderer.CurrentFrameSize;
  public Box2 Bounds => new(Position - Size * 0.5f, Position + Size * 0.5f);

  public virtual void Update(double deltaSeconds)
    => SpriteRenderer.Update(deltaSeconds);

  public virtual void Draw(Shader shader)
  {
    if (shader is null) throw new ArgumentNullException(nameof(shader));

    var translation = Matrix4.CreateTranslation(Position.X, Position.Y, 0f);
    shader.SetMatrix4("uModel", translation);
    SpriteRenderer.Draw();
  }

  public virtual void Dispose()
    => SpriteRenderer.Dispose();
}

public sealed class Player : SceneObject
{
  private string _activeAnimation = string.Empty;

  public Player(Vector2 position, SpriteRenderer spriteRenderer)
      : base(position, spriteRenderer)
  {
  }

  public bool IsGrounded { get; set; }

  public void PlayAnimation(string animationName)
  {
    if (string.Equals(_activeAnimation, animationName, StringComparison.Ordinal))
    {
      return;
    }

    SpriteRenderer.SetAnimation(animationName, restart: true);
    _activeAnimation = animationName;
  }
}

public sealed class GroundTile : SceneObject
{
  public GroundTile(Vector2 position, SpriteRenderer spriteRenderer)
      : base(position, spriteRenderer)
  {
    Velocity = Vector2.Zero;
  }
}
