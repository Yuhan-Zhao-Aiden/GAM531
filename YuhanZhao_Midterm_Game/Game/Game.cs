using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Monolith
{
  public class Game : GameWindow
  {
    

    public Game(GameWindowSettings gs, NativeWindowSettings ns)
    : base(gs, ns) { }

    protected override void OnLoad()
    {
      base.OnLoad();

    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);
    }

    protected override void OnUnload()
    {
      base.OnUnload();
    }
  }
}