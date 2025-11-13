using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using _2DCollision.Rendering;

namespace _2DCollision;

internal static class Program
{
  public static void Main()
  {
    var nativeSettings = new NativeWindowSettings
    {
      Title = "2D Collision AABB vs Circle",
      Size = new Vector2i(900, 600),
      Flags = ContextFlags.ForwardCompatible
    };

    using var game = new CollisionGame(GameWindowSettings.Default, nativeSettings);
    game.Run();
  }
}

internal sealed class CollisionGame : GameWindow
{
  private ShapeRenderer _renderer = null!;
  private Scene _scene = null!;
  private Matrix4 _projection;

  public CollisionGame(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings)
      : base(gameSettings, nativeSettings)
  {
  }

  protected override void OnLoad()
  {
    base.OnLoad();
    GL.ClearColor(0.08f, 0.08f, 0.12f, 1f);
    GL.Viewport(0, 0, Size.X, Size.Y);

    _renderer = new ShapeRenderer();
    _scene = Scene.CreateDefault(Size.X, Size.Y);
    _projection = Matrix4.CreateOrthographicOffCenter(0f, Size.X, Size.Y, 0f, -1f, 1f);
  }

  protected override void OnUpdateFrame(FrameEventArgs args)
  {
    base.OnUpdateFrame(args);
    _scene.Update((float)args.Time);
  }

  protected override void OnRenderFrame(FrameEventArgs args)
  {
    base.OnRenderFrame(args);

    GL.Clear(ClearBufferMask.ColorBufferBit);
    _scene.Render(_renderer, _projection);

    SwapBuffers();
  }

  protected override void OnResize(ResizeEventArgs e)
  {
    base.OnResize(e);

    GL.Viewport(0, 0, Size.X, Size.Y);
    _projection = Matrix4.CreateOrthographicOffCenter(0f, Size.X, Size.Y, 0f, -1f, 1f);
    _scene.OnViewportChanged(Size.X, Size.Y);
  }

  protected override void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
    {
      return;
    }

    _renderer?.Dispose();
  }
}
