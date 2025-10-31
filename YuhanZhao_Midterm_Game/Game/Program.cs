using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace Monolith
{
  public static class Program {
    public static void Main()
    {
      var gs = new GameWindowSettings()
      {
        UpdateFrequency = 120
      };

      var ns = new NativeWindowSettings
      {
        Title = "Monolith",
        ClientSize = new Vector2i(1920, 1080),
      };

      using (Game game = new(gs, ns))
      {
        game.Run();
      }
    }

  }
}