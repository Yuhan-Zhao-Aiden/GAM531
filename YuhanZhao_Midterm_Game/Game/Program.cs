using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace Monolith
{
  public static class Program {
    public static void Main()
    {
      var ns = new NativeWindowSettings
      {
        Title = "Monolith",
        ClientSize = new Vector2i(1280, 720),
      };

      using (Game game = new(GameWindowSettings.Default, ns))
      {
        game.Run();
      }
    }

  }
}