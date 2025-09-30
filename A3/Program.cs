// See https://aka.ms/new-console-template for more information
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace CubeRender
{
  class Program
  {
    static void Main(string[] args)
    {
      var nws = new NativeWindowSettings()
      {
        ClientSize = new Vector2i(1080, 720),
        Title = "My Cube!"
      };

      using (var game = new Game(GameWindowSettings.Default, nws))
      {
        game.Run();
      }
    }
  }
}