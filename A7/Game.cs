using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SixLabors.ImageSharp;
using ImageSharpImage = SixLabors.ImageSharp.Image;

namespace knight;

public sealed class Game : GameWindow
{
  private readonly string _contentRoot;

  private Shader? _shader;
  private Player? _player;
  private readonly List<GroundTile> _groundTiles = new();
  private string? _groundTexturePath;
  private Vector2i _groundTextureSize;

  private Matrix4 _projection;
  private Matrix4 _view = Matrix4.Identity;

  private const float Gravity = -2000f;
  private const float JumpImpulse = 750f;
  private const float HorizontalDamping = 12f;
  private const double MaxDeltaSeconds = 1d / 60d;

  private const string VertexShaderSource = @"
#version 330 core
layout(location = 0) in vec2 aPosition;
layout(location = 1) in vec2 aTexCoord;

uniform mat4 uProjection;
uniform mat4 uView;
uniform mat4 uModel;

out vec2 vTexCoord;

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(aPosition.xy, 0.0, 1.0);
    vTexCoord = aTexCoord;
}";

  private const string FragmentShaderSource = @"
#version 330 core
in vec2 vTexCoord;

out vec4 FragColor;

uniform sampler2D uTexture;

void main()
{
    FragColor = texture(uTexture, vTexCoord);
}";

  public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, string? contentRoot = null)
      : base(gameWindowSettings, nativeWindowSettings)
  {
    _contentRoot = contentRoot ?? AppContext.BaseDirectory;
  }

  protected override void OnLoad()
  {
    base.OnLoad();

    GL.ClearColor(0.1f, 0.12f, 0.16f, 1f);
    GL.Enable(EnableCap.Blend);
    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

    _shader = new Shader(VertexShaderSource, FragmentShaderSource);
    _shader.Use();
    _shader.SetInt("uTexture", 0);

    UpdateProjection();
    _shader.SetMatrix4("uView", _view);
    _shader.SetMatrix4("uProjection", _projection);

    var playerRenderer = new SpriteRenderer();

    var idleSpritePath = RequireAsset(Path.Combine(_contentRoot, "Animation", "_Idle.png"), "Idle sprite sheet is missing.");
    var jumpSpritePath = RequireAsset(Path.Combine(_contentRoot, "Animation", "_Jump.png"), "Jump sprite sheet is missing.");
    var fallSpritePath = RequireAsset(Path.Combine(_contentRoot, "Animation", "_Fall.png"), "Fall sprite sheet is missing.");

    var idleFrameOrigins = new List<Vector2i>(10);
    for (var i = 0; i < 10; i++)
    {
      idleFrameOrigins.Add(new Vector2i(40 + i * 120, 0));
    }

    var jumpFrameOrigins = new List<Vector2i>(3);
    var fallFrameOrigins = new List<Vector2i>(3);
    for (var i = 0; i < 3; i++)
    {
      var x = 40 + i * 120;
      jumpFrameOrigins.Add(new Vector2i(x, 0));
      fallFrameOrigins.Add(new Vector2i(x, 0));
    }

    playerRenderer.LoadAnimation("Idle", idleSpritePath, frameWidth: 25, frameHeight: 40, idleFrameOrigins, frameDurationSeconds: 0.1, loop: true);
    playerRenderer.LoadAnimation("Jump", jumpSpritePath, frameWidth: 30, frameHeight: 40, jumpFrameOrigins, frameDurationSeconds: 0.1, loop: false);
    playerRenderer.LoadAnimation("Fall", fallSpritePath, frameWidth: 30, frameHeight: 40, fallFrameOrigins, frameDurationSeconds: 0.1, loop: true);

    var playerStart = new Vector2(ClientSize.X / 2f, ClientSize.Y / 2f);
    _player = new Player(playerStart, playerRenderer);
    _player.PlayAnimation("Idle");

    var groundSpritePath = RequireAsset(Path.Combine(_contentRoot, "Assets", "ground.png"), "Ground texture is missing.");
    _groundTexturePath = groundSpritePath;
    _groundTextureSize = IdentifyTextureSize(groundSpritePath);
    BuildGroundTiles();
  }

  protected override void OnUpdateFrame(FrameEventArgs args)
  {
    base.OnUpdateFrame(args);

    if (_player is null)
    {
      return;
    }

    var deltaSeconds = Math.Min(args.Time, MaxDeltaSeconds);

    HandlePlayerInput(_player);
    IntegratePlayer(_player, deltaSeconds);
    ResolvePlayerGroundCollisions(_player);
    UpdatePlayerAnimation(_player);
    _player.Update(deltaSeconds);

    foreach (var tile in _groundTiles)
    {
      tile.Update(deltaSeconds);
    }
  }

  protected override void OnRenderFrame(FrameEventArgs args)
  {
    base.OnRenderFrame(args);

    GL.Clear(ClearBufferMask.ColorBufferBit);

    if (_shader is null || _player is null)
    {
      SwapBuffers();
      return;
    }

    _shader.Use();

    foreach (var tile in _groundTiles)
    {
      tile.Draw(_shader);
    }

    _player.Draw(_shader);

    SwapBuffers();
  }

  protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
  {
    base.OnFramebufferResize(e);

    GL.Viewport(0, 0, e.Width, e.Height);
    UpdateProjection();

    if (_shader is not null)
    {
      _shader.Use();
      _shader.SetMatrix4("uProjection", _projection);
    }

    if (_player is not null)
    {
      _player.Position = new Vector2(e.Width / 2f, e.Height / 2f);
    }

    BuildGroundTiles();
  }

  protected override void OnUnload()
  {
    _player?.Dispose();

    foreach (var tile in _groundTiles)
    {
      tile.Dispose();
    }

    _shader?.Dispose();
    base.OnUnload();
  }

  private void UpdateProjection()
      => _projection = Matrix4.CreateOrthographicOffCenter(0, ClientSize.X, 0, ClientSize.Y, -1f, 1f);

  private static string RequireAsset(string path, string errorMessage)
  {
    if (File.Exists(path))
    {
      return path;
    }

    throw new FileNotFoundException(errorMessage, path);
  }

  private static Vector2i IdentifyTextureSize(string filePath)
  {
    var info = ImageSharpImage.Identify(filePath);
    if (info is null)
    {
      throw new InvalidOperationException($"Unable to identify texture dimensions for '{filePath}'.");
    }

    return new Vector2i(info.Width, info.Height);
  }

  private void BuildGroundTiles()
  {
    foreach (var tile in _groundTiles)
    {
      tile.Dispose();
    }
    _groundTiles.Clear();

    if (_groundTexturePath is null || _groundTextureSize == Vector2i.Zero)
    {
      return;
    }

    var tileWidth = _groundTextureSize.X;
    var tileHeight = _groundTextureSize.Y;

    if (tileWidth <= 0 || tileHeight <= 0)
    {
      return;
    }

    var tilesNeeded = (int)Math.Ceiling(ClientSize.X / (float)tileWidth) + 1;

    for (var i = 0; i < tilesNeeded; i++)
    {
      var renderer = new SpriteRenderer();
      renderer.LoadAnimation("Static", _groundTexturePath, tileWidth, tileHeight, frameDurationSeconds: 0, loop: true);
      renderer.SetAnimation("Static", true);

      var x = i * tileWidth + tileWidth / 2f;
      var position = new Vector2(x, tileHeight / 2f);
      var tile = new GroundTile(position, renderer);
      _groundTiles.Add(tile);
    }
  }

  private void HandlePlayerInput(Player player)
  {
    var keyboard = KeyboardState;

    var wantsJump = keyboard.IsKeyPressed(Keys.Space) || keyboard.IsKeyPressed(Keys.W) || keyboard.IsKeyPressed(Keys.Up);
    if (wantsJump && player.IsGrounded)
    {
      player.Velocity = new Vector2(player.Velocity.X, JumpImpulse);
      player.IsGrounded = false;
    }
  }

  private void IntegratePlayer(Player player, double deltaSeconds)
  {
    var velocity = player.Velocity;

    velocity.Y += Gravity * (float)deltaSeconds;

    var dampingFactor = Math.Clamp(HorizontalDamping * (float)deltaSeconds, 0f, 1f);
    velocity.X = MathHelper.Lerp(velocity.X, 0f, dampingFactor);

    player.Velocity = velocity;
    player.Position += velocity * (float)deltaSeconds;
  }

  private void ResolvePlayerGroundCollisions(Player player)
  {
    player.IsGrounded = false;

    if (_groundTiles.Count == 0)
    {
      return;
    }

    var playerHalfSize = player.Size * 0.5f;
    var playerHalfWidth = playerHalfSize.X;
    var playerHalfHeight = playerHalfSize.Y;

    foreach (var tile in _groundTiles)
    {
      var tileHalfSize = tile.Size * 0.5f;
      var tileHalfWidth = tileHalfSize.X;
      var tileHalfHeight = tileHalfSize.Y;

      var playerLeft = player.Position.X - playerHalfWidth;
      var playerRight = player.Position.X + playerHalfWidth;
      var tileLeft = tile.Position.X - tileHalfWidth;
      var tileRight = tile.Position.X + tileHalfWidth;

      if (playerRight <= tileLeft || playerLeft >= tileRight)
      {
        continue;
      }

      var playerBottom = player.Position.Y - playerHalfHeight;
      var tileTop = tile.Position.Y + tileHalfHeight;
      var tileBottom = tile.Position.Y - tileHalfHeight;

      if (playerBottom < tileTop && playerBottom >= tileBottom && player.Velocity.Y <= 0f)
      {
        var penetration = tileTop - playerBottom;
        player.Position = new Vector2(player.Position.X, player.Position.Y + penetration);
        player.Velocity = new Vector2(player.Velocity.X, 0f);
        player.IsGrounded = true;
        break;
      }
    }
  }

  private void UpdatePlayerAnimation(Player player)
  {
    const float idleSpeedThreshold = 5f;

    if (!player.IsGrounded)
    {
      player.PlayAnimation(player.Velocity.Y >= 0 ? "Jump" : "Fall");
      return;
    }

    if (MathF.Abs(player.Velocity.X) <= idleSpeedThreshold)
    {
      player.PlayAnimation("Idle");
      return;
    }

    player.PlayAnimation("Idle");
  }
}
