using A9.Components;
using A9.Core;
using A9.Graphics;
using OpenTK.Mathematics;

namespace A9.GameObjects;

public class Ground : GameObject
{
  public Ground(Vector3 position, Vector3 size, Material material, float uvScale = 1.0f) : base()
  {
    Transform.Position = position;
    Transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);

    var mesh = MeshGenerator.GeneratePlane(size.X, uvScale);
    var meshRenderer = new MeshRenderer(mesh, material);
    AddComponent(meshRenderer);

    var collider = new BoxCollider(size, "Ground");
    AddComponent(collider);

    var rigidBody = new RigidBody(useGravity: false, isKinematic: true);
    AddComponent(rigidBody);

    Physics.PhysicsSystem.RegisterCollider(collider);
    Physics.PhysicsSystem.RegisterRigidBody(rigidBody);
  }
}
