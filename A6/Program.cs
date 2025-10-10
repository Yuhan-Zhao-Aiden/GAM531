// See https://aka.ms/new-console-template for more information
using PhongLighting;
using OpenTK.Windowing.Desktop;

public static class Program
{
  public static void Main()
  {
    var gs = new GameWindowSettings
    {
      UpdateFrequency = 60
    };
    var ns = new NativeWindowSettings
    {
      Title = "A6 - Camera Movement",
      APIVersion = new Version(3, 3),
      ClientSize = new OpenTK.Mathematics.Vector2i(1280, 720),
      StartVisible = true,
      StartFocused = true,
      Profile = OpenTK.Windowing.Common.ContextProfile.Core
    };

    using var game = new Game(gs, ns);
    game.Run();
  }
}