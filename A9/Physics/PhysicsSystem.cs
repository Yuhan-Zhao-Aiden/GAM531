using A9.Components;
using A9.Core;

namespace A9.Physics;

public static class PhysicsSystem
{
  private static List<BoxCollider> _colliders = new List<BoxCollider>();
  private static List<RigidBody> _rigidBodies = new List<RigidBody>();

  public static void RegisterCollider(BoxCollider collider)
  {
    if (!_colliders.Contains(collider))
    {
      _colliders.Add(collider);
    }
  }

  public static void UnregisterCollider(BoxCollider collider)
  {
    _colliders.Remove(collider);
  }

  public static void RegisterRigidBody(RigidBody rigidBody)
  {
    if (!_rigidBodies.Contains(rigidBody))
    {
      _rigidBodies.Add(rigidBody);
    }
  }

  public static void UnregisterRigidBody(RigidBody rigidBody)
  {
    _rigidBodies.Remove(rigidBody);
  }

  public static void Update(float deltaTime)
  {
    foreach (var rigidBody in _rigidBodies)
    {
      if (rigidBody.GameObject == null || rigidBody.IsKinematic) continue;

      if (rigidBody.UseGravity)
      {
        rigidBody.Velocity += new OpenTK.Mathematics.Vector3(0, rigidBody.Gravity * deltaTime, 0);
      }

      rigidBody.Velocity *= (1.0f - rigidBody.Drag * deltaTime);
    }

    foreach (var rigidBody in _rigidBodies)
    {
      if (rigidBody.GameObject == null || rigidBody.IsKinematic) continue;
      rigidBody.GameObject.Transform.Position += rigidBody.Velocity * deltaTime;
    }

    CheckCollisions();
  }

  private static void CheckCollisions()
  {
    for (int i = 0; i < _colliders.Count; i++)
    {
      for (int j = i + 1; j < _colliders.Count; j++)
      {
        var colliderA = _colliders[i];
        var colliderB = _colliders[j];

        if (colliderA.GameObject == null || colliderB.GameObject == null) continue;
        if (!colliderA.GameObject.IsActive || !colliderB.GameObject.IsActive) continue;

        if (colliderA.Intersects(colliderB))
        {

          colliderA.GameObject.OnCollision(colliderB.GameObject);
          colliderB.GameObject.OnCollision(colliderA.GameObject);

          if (!colliderA.IsTrigger && !colliderB.IsTrigger)
          {
            ResolveCollision(colliderA, colliderB);
          }
        }
      }
    }
  }

  private static void ResolveCollision(BoxCollider colliderA, BoxCollider colliderB)
  {
    var rbA = colliderA.GameObject?.GetComponent<RigidBody>();
    var rbB = colliderB.GameObject?.GetComponent<RigidBody>();

    if ((rbA == null || rbA.IsKinematic) && (rbB == null || rbB.IsKinematic))
      return;

    var aabbA = colliderA.GetWorldAABB();
    var aabbB = colliderB.GetWorldAABB();

    float overlapX = Math.Min(aabbA.Max.X - aabbB.Min.X, aabbB.Max.X - aabbA.Min.X);
    float overlapY = Math.Min(aabbA.Max.Y - aabbB.Min.Y, aabbB.Max.Y - aabbA.Min.Y);
    float overlapZ = Math.Min(aabbA.Max.Z - aabbB.Min.Z, aabbB.Max.Z - aabbA.Min.Z);

    if (overlapX < overlapY && overlapX < overlapZ)
    {
      // Separate on X axis
      float direction = aabbA.Center.X < aabbB.Center.X ? -1 : 1;
      SeparateObjects(colliderA, colliderB, new OpenTK.Mathematics.Vector3(direction * overlapX, 0, 0), rbA, rbB);
    }
    else if (overlapY < overlapZ)
    {
      float direction = aabbA.Center.Y < aabbB.Center.Y ? -1 : 1;
      SeparateObjects(colliderA, colliderB, new OpenTK.Mathematics.Vector3(0, direction * overlapY, 0), rbA, rbB);


      if (direction > 0 && rbA != null && !rbA.IsKinematic && rbA.Velocity.Y < 0)
      {
        rbA.Velocity = new OpenTK.Mathematics.Vector3(rbA.Velocity.X, 0, rbA.Velocity.Z);
      }
      else if (direction < 0 && rbB != null && !rbB.IsKinematic && rbB.Velocity.Y < 0)
      {
        rbB.Velocity = new OpenTK.Mathematics.Vector3(rbB.Velocity.X, 0, rbB.Velocity.Z);
      }
    }
    else
    {
      float direction = aabbA.Center.Z < aabbB.Center.Z ? -1 : 1;
      SeparateObjects(colliderA, colliderB, new OpenTK.Mathematics.Vector3(0, 0, direction * overlapZ), rbA, rbB);
    }
  }

  private static void SeparateObjects(BoxCollider colliderA, BoxCollider colliderB,
      OpenTK.Mathematics.Vector3 separation, RigidBody? rbA, RigidBody? rbB)
  {
    bool aIsKinematic = rbA == null || rbA.IsKinematic;
    bool bIsKinematic = rbB == null || rbB.IsKinematic;

    const float epsilon = 0.001f;
    if (separation.LengthSquared > 0)
    {
      separation = separation.Normalized() * (separation.Length + epsilon);
    }

    if (!aIsKinematic && bIsKinematic)
    {
      // Only A moves
      colliderA.GameObject!.Transform.Position += separation;
    }
    else if (aIsKinematic && !bIsKinematic)
    {
      // Only B moves
      colliderB.GameObject!.Transform.Position -= separation;
    }
    else if (!aIsKinematic && !bIsKinematic)
    {
      colliderA.GameObject!.Transform.Position += separation * 0.5f;
      colliderB.GameObject!.Transform.Position -= separation * 0.5f;
    }
  }

  public static void Clear()
  {
    _colliders.Clear();
    _rigidBodies.Clear();
  }
}
