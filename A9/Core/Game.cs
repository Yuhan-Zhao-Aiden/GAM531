using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using A9.Graphics;
using A9.Components;
using A9.GameObjects;

namespace A9.Core;

public class Game : GameWindow
{
  private Shader? _shader;
  private Camera? _camera;
  private Material? _playerMaterial;
  private Material? _enemyMaterial;
  private Material? _groundMaterial;
  private Texture? _groundTexture;

  private Player? _player;
  private Ground? _ground;
  private List<Enemy> _enemies = new List<Enemy>();

  private Vector3 _lightPosition = new Vector3(5.0f, 8.0f, 5.0f);
  private Vector3 _lightColor = new Vector3(1.0f, 1.0f, 1.0f);

  private Vector2 _lastMousePos;
  private bool _firstMouseMove = true;
  private bool winMessage = false;

  public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
      : base(gameWindowSettings, nativeWindowSettings)
  {
  }

  protected override void OnLoad()
  {
    base.OnLoad();

    GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);

    GL.Enable(EnableCap.DepthTest);

    GL.Disable(EnableCap.CullFace);
    // GL.Enable(EnableCap.CullFace);
    // GL.CullFace(TriangleFace.Back);

    CursorState = CursorState.Grabbed;

    _shader = new Shader("Shaders/vertex.glsl", "Shaders/fragment.glsl");

    _groundTexture = new Texture("Assets/ground.jpg");

    _camera = new Camera(
        new Vector3(0.0f, 5.0f, 10.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        Size.X / (float)Size.Y
    );

    _playerMaterial = new Material(_shader)
    {
      Color = new Vector3(0.2f, 0.8f, 0.2f),
      LightPosition = _lightPosition,
      LightColor = _lightColor
    };

    _enemyMaterial = new Material(_shader)
    {
      Color = new Vector3(0.9f, 0.2f, 0.2f),
      LightPosition = _lightPosition,
      LightColor = _lightColor
    };

    _groundMaterial = new Material(_shader, _groundTexture)
    {
      Color = Vector3.One,
      LightPosition = _lightPosition,
      LightColor = _lightColor
    };

    _player = new Player(_playerMaterial);
    _player.Transform.Position = new Vector3(0, 3, 0);
    _player.OnDeath += RestartGame;

    _camera.FollowTarget = _player;

    CreateEnemies();

    _ground = new Ground(
        new Vector3(0, -0.1f, 0),
        new Vector3(100, 0.1f, 100),
        _groundMaterial,
        uvScale: 4.0f
    );

    Console.WriteLine("===== 3D Platformer =====");
    Console.WriteLine("WASD: Move | Space: Jump | Mouse: Look around | ESC: Exit");
  }

  protected override void OnUpdateFrame(FrameEventArgs args)
  {
    base.OnUpdateFrame(args);

    float deltaTime = (float)args.Time;

    if (!_firstMouseMove && _camera != null)
    {
      var mousePos = new Vector2(MouseState.X, MouseState.Y);
      var delta = mousePos - _lastMousePos;
      _camera.ProcessMouseMovement(delta.X, delta.Y);
      _lastMousePos = mousePos;
    }
    else
    {
      _lastMousePos = new Vector2(MouseState.X, MouseState.Y);
      _firstMouseMove = false;
    }

    if (_player != null && _camera != null)
    {
      _player.HandleInput(KeyboardState, deltaTime, _camera.GetYaw());
    }

    Physics.PhysicsSystem.Update(deltaTime);

    _camera?.Update(deltaTime);

    _player?.Update(deltaTime);
    _ground?.Update(deltaTime);

    foreach (var enemy in _enemies.ToList())
    {
      if (enemy.IsActive)
      {
        enemy.Update(deltaTime);
      }
      else
      {
        _enemies.Remove(enemy);
        enemy.GetComponent<MeshRenderer>()?.Mesh?.Dispose();
      }
    }

    if (_enemies.Count == 0 && !GameState.getInstance().win)
    {
      GameState.getInstance().incrementLevel();

      if (!GameState.getInstance().win)
      {
        CreateEnemies();
      }
      else
      {
        if (!winMessage)
        {
          Console.WriteLine("You win");
          winMessage = true;
        }
      }
    }

    if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
    {
      Close();
    }
  }

  protected override void OnRenderFrame(FrameEventArgs args)
  {
    base.OnRenderFrame(args);

    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    if (_camera != null)
    {
      var viewMatrix = _camera.GetViewMatrix();
      var projectionMatrix = _camera.GetProjectionMatrix();

      if (_playerMaterial != null)
      {
        _playerMaterial.ViewMatrix = viewMatrix;
        _playerMaterial.ProjectionMatrix = projectionMatrix;
        _playerMaterial.ViewPosition = _camera.Position;
        _playerMaterial.LightPosition = _lightPosition;
        _playerMaterial.LightColor = _lightColor;
      }

      if (_enemyMaterial != null)
      {
        _enemyMaterial.ViewMatrix = viewMatrix;
        _enemyMaterial.ProjectionMatrix = projectionMatrix;
        _enemyMaterial.ViewPosition = _camera.Position;
        _enemyMaterial.LightPosition = _lightPosition;
        _enemyMaterial.LightColor = _lightColor;
      }

      if (_groundMaterial != null)
      {
        _groundMaterial.ViewMatrix = viewMatrix;
        _groundMaterial.ProjectionMatrix = projectionMatrix;
        _groundMaterial.ViewPosition = _camera.Position;
        _groundMaterial.LightPosition = _lightPosition;
        _groundMaterial.LightColor = _lightColor;
      }
    }

    // Render game objects
    _ground?.Render();

    foreach (var enemy in _enemies)
    {
      enemy.Render();
    }

    _player?.Render();

    // Render UI text using window title for now 
    Title = $"3D Platformer - Score: {GameState.getInstance().score}  Level: {GameState.getInstance().level}" +
            (GameState.getInstance().win ? "  YOU WIN!" : "");

    SwapBuffers();
  }

  protected override void OnResize(ResizeEventArgs e)
  {
    base.OnResize(e);

    GL.Viewport(0, 0, e.Width, e.Height);

    _camera?.UpdateAspectRatio(e.Width / (float)e.Height);
  }

  private void CreateEnemies() // Create enemy base on wave, +1 every wave
  {
    if (_player == null || _enemyMaterial == null) return;
    int numEnemy = GameState.getInstance().level + 3;

    for (int i = 0; i < numEnemy; i++)
    {
      var enemy = new Enemy(_enemyMaterial, _player);
      enemy.Transform.Position = new Vector3(
        (float)RNG.getRandomDouble(-30, 30),
        2,
        (float)RNG.getRandomDouble(-30, 30)
      );
      _enemies.Add(enemy);
    }

  }

  private void RestartGame()
  {
    Console.WriteLine("You died! Restarting game...");

    Physics.PhysicsSystem.Clear();

    foreach (var enemy in _enemies)
    {
      enemy.GetComponent<MeshRenderer>()?.Mesh?.Dispose();
    }
    _enemies.Clear();

    _player?.GetComponent<MeshRenderer>()?.Mesh?.Dispose();

    if (_playerMaterial != null)
    {
      _player = new Player(_playerMaterial);
      _player.Transform.Position = new Vector3(0, 3, 0);
      _player.OnDeath += RestartGame;

      if (_camera != null)
      {
        _camera.FollowTarget = _player;
      }
    }

    CreateEnemies();

    if (_ground != null)
    {
      var groundCollider = _ground.GetComponent<BoxCollider>();
      var groundRigidBody = _ground.GetComponent<RigidBody>();
      if (groundCollider != null)
        Physics.PhysicsSystem.RegisterCollider(groundCollider);
      if (groundRigidBody != null)
        Physics.PhysicsSystem.RegisterRigidBody(groundRigidBody);
    }
  }

  protected override void OnUnload()
  {
    base.OnUnload();

    _shader?.Dispose();
    _groundTexture?.Dispose();

    // Clean up meshes
    _player?.GetComponent<MeshRenderer>()?.Mesh?.Dispose();
    _ground?.GetComponent<MeshRenderer>()?.Mesh?.Dispose();

    foreach (var enemy in _enemies)
    {
      enemy.GetComponent<MeshRenderer>()?.Mesh?.Dispose();
    }

    Physics.PhysicsSystem.Clear();

  }
}