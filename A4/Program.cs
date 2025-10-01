using ApplyTexture;
using OpenTK.Windowing.Desktop;

public static class Program
{
  public static void Main()
  {
    var ns = new NativeWindowSettings
    {
      Title = "A4 - Apply texture",
      ClientSize = new OpenTK.Mathematics.Vector2i(1280, 720),
      StartVisible = true,
      StartFocused = true
    };

    using var game = new Game(
      GameWindowSettings.Default,
      ns
    );
    game.Run();
  }
}