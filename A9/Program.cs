using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using A9.Core;

class Program
{
  public static void Main()
  {
    var gameWindowSettings = GameWindowSettings.Default;
    gameWindowSettings.UpdateFrequency = 60.0;

    var nativeWindowSettings = new NativeWindowSettings()
    {
      ClientSize = new Vector2i(1280, 720),
      Title = "3D Platformer ",
      WindowBorder = WindowBorder.Fixed,
      StartVisible = true,
      StartFocused = true,
      API = ContextAPI.OpenGL,
      Profile = ContextProfile.Core,
      APIVersion = new Version(3, 3)
    };

    try
    {
      using var game = new Game(gameWindowSettings, nativeWindowSettings);
      game.Run();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error: {ex.Message}");
      Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
  }
}