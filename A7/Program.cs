using OpenTK.Windowing.Desktop;

using knight;

var gameWindowSettings = GameWindowSettings.Default;
var nativeWindowSettings = new NativeWindowSettings
{
    Title = "Idle Animation MVP",
    ClientSize = (1080, 720)
};

using var game = new Game(gameWindowSettings, nativeWindowSettings);
game.Run();
