using A9.Components;
using A9.Core;
using A9.Graphics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace A9.GameObjects;

public class Player : GameObject
{
  private RigidBody? _rigidBody;
  private BoxCollider? _collider;

  public float MoveSpeed { get; set; } = 8.0f;
  public float JumpForce { get; set; } = 10.0f;
  public float RotationSpeed { get; set; } = 10.0f;

  private bool _isGrounded = false;
  private float _yaw = 0.0f;

  public Player(Material material) : base()
  {
    var mesh = MeshGenerator.GenerateCube();
    var meshRenderer = new MeshRenderer(mesh, material);
    AddComponent(meshRenderer);


    _collider = new BoxCollider(new Vector3(0.95f, 0.95f, 0.95f), "Player");
    _rigidBody = new RigidBody(useGravity: true);
    AddComponent(_collider);
    AddComponent(_rigidBody);

    Physics.PhysicsSystem.RegisterCollider(_collider);
    Physics.PhysicsSystem.RegisterRigidBody(_rigidBody);
  }

  public void HandleInput(KeyboardState keyboard, float deltaTime, float cameraYaw)
  {
    if (_rigidBody == null) return;

    Vector3 moveDirection = Vector3.Zero;

    Vector3 forward = new Vector3(-(float)Math.Sin(cameraYaw), 0, -(float)Math.Cos(cameraYaw));
    Vector3 right = new Vector3((float)Math.Cos(cameraYaw), 0, -(float)Math.Sin(cameraYaw));

    if (keyboard.IsKeyDown(Keys.W))
      moveDirection += forward;
    if (keyboard.IsKeyDown(Keys.S))
      moveDirection -= forward;
    if (keyboard.IsKeyDown(Keys.A))
      moveDirection -= right;
    if (keyboard.IsKeyDown(Keys.D))
      moveDirection += right;

    if (moveDirection.LengthSquared > 0.001f)
    {
      moveDirection.Normalize();

      _rigidBody.Velocity = new Vector3(
          moveDirection.X * MoveSpeed,
          _rigidBody.Velocity.Y,
          moveDirection.Z * MoveSpeed
      );

      if (moveDirection.LengthSquared > 0.001f)
      {
        float targetYaw = (float)Math.Atan2(moveDirection.X, moveDirection.Z);
        _yaw = MathHelper.Lerp(_yaw, targetYaw, RotationSpeed * deltaTime);
        Transform.Rotation = new Vector3(0, _yaw, 0);
      }
    }
    else
    {
      _rigidBody.Velocity = new Vector3(0, _rigidBody.Velocity.Y, 0);
    }

    // Jumping
    if (keyboard.IsKeyDown(Keys.Space) && _isGrounded)
    {
      _rigidBody.Velocity = new Vector3(
          _rigidBody.Velocity.X,
          JumpForce,
          _rigidBody.Velocity.Z
      );
      _isGrounded = false;
    }
  }

  public override void OnCollision(GameObject other)
  {
    base.OnCollision(other);

    // Check if  landed on ground
    if (other.GetComponent<BoxCollider>()?.Tag == "Ground")
    {
      if (Transform.Position.Y > other.Transform.Position.Y)
      {
        _isGrounded = true;
      }
    }

    // Check collision with enemy
    if (other.GetComponent<BoxCollider>()?.Tag == "Enemy")
    {
      var enemy = other as Enemy;
      if (enemy != null)
      {
        // Check if player is above enemy (stomping)
        if (Transform.Position.Y > other.Transform.Position.Y + 0.3f && _rigidBody != null && _rigidBody.Velocity.Y < 0)
        {
          enemy.Destroy();
          GameState.getInstance().incrementScore();
          Console.WriteLine($"Your score: {GameState.getInstance().score}");
          _rigidBody.Velocity = new Vector3(_rigidBody.Velocity.X, JumpForce * 0.5f, _rigidBody.Velocity.Z);
        }
        else
        {
          // Get hit by enemy
          GameState.getInstance().reset();
          OnDeath?.Invoke();
        }
      }
    }
  }

  public event Action? OnDeath;

  public float GetYaw() => _yaw;
}
