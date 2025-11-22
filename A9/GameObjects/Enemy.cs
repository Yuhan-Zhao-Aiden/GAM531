using A9.Components;
using A9.Core;
using A9.Graphics;
using OpenTK.Mathematics;

namespace A9.GameObjects;

public class Enemy : GameObject
{
  private RigidBody? _rigidBody;
  private BoxCollider? _collider;
  private Player? _targetPlayer;

  public float MoveSpeed { get; set; } = 2.0f;

  public Enemy(Material material, Player targetPlayer) : base()
  {
    _targetPlayer = targetPlayer;

    var mesh = MeshGenerator.GenerateCube();
    var meshRenderer = new MeshRenderer(mesh, material);
    AddComponent(meshRenderer);

    Transform.Scale = new Vector3(1.2f, 0.6f, 1.2f);

    _collider = new BoxCollider(new Vector3(1.2f, 0.6f, 1.2f), "Enemy");
    _rigidBody = new RigidBody(useGravity: true);
    AddComponent(_collider);
    AddComponent(_rigidBody);

    Physics.PhysicsSystem.RegisterCollider(_collider);
    Physics.PhysicsSystem.RegisterRigidBody(_rigidBody);
  }

  public override void Update(float deltaTime)
  {
    base.Update(deltaTime);

    if (!base.IsActive || _targetPlayer == null || _rigidBody == null) return;

    Vector3 directionToPlayer = _targetPlayer.Transform.Position - Transform.Position;
    directionToPlayer.Y = 0;

    if (directionToPlayer.LengthSquared > 0.1f)
    {
      directionToPlayer.Normalize();

      // Move towards 
      _rigidBody.Velocity = new Vector3(
          directionToPlayer.X * MoveSpeed,
          _rigidBody.Velocity.Y,
          directionToPlayer.Z * MoveSpeed
      );

      float targetYaw = (float)Math.Atan2(directionToPlayer.X, directionToPlayer.Z);
      Transform.Rotation = new Vector3(0, targetYaw, 0);
    }
  }

  public void Destroy()
  {
    base.IsActive = false;

    if (_collider != null)
      Physics.PhysicsSystem.UnregisterCollider(_collider);
    if (_rigidBody != null)
      Physics.PhysicsSystem.UnregisterRigidBody(_rigidBody);
  }
}