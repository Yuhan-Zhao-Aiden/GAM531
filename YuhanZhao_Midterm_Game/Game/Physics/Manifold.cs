// https://research.ncl.ac.uk/game/mastersdegree/gametechnologies/previousinformation/physics5collisionmanifolds/2017%20Tutorial%205%20-%20Collision%20Manifolds.pdf
using OpenTK.Mathematics;

namespace Monolith.Physics
{
  public struct Manifold
  {
    public PhysicsBody A;
    public PhysicsBody B;
    public Vector3 Normal;
    public float Penetration;
    public bool IsValid => Penetration > 0f && Normal.LengthSquared > 0f;
  }
}