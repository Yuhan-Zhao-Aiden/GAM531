using OpenTK.Mathematics;

namespace A9.Physics;

public struct AABB
{
  public Vector3 Min { get; set; }
  public Vector3 Max { get; set; }

  public AABB(Vector3 min, Vector3 max)
  {
    Min = min;
    Max = max;
  }

  public static AABB FromCenterAndSize(Vector3 center, Vector3 size)
  {
    Vector3 halfSize = size * 0.5f;
    return new AABB(center - halfSize, center + halfSize);
  }

  public Vector3 Center => (Min + Max) * 0.5f;
  public Vector3 Size => Max - Min;

  public bool Intersects(AABB other)
  {
    return (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
           (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
           (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);
  }

  public bool Contains(Vector3 point)
  {
    return point.X >= Min.X && point.X <= Max.X &&
           point.Y >= Min.Y && point.Y <= Max.Y &&
           point.Z >= Min.Z && point.Z <= Max.Z;
  }

  public AABB Expand(float amount)
  {
    Vector3 expansion = new Vector3(amount);
    return new AABB(Min - expansion, Max + expansion);
  }

  public static AABB Merge(AABB a, AABB b)
  {
    return new AABB(
        new Vector3(MathF.Min(a.Min.X, b.Min.X), MathF.Min(a.Min.Y, b.Min.Y), MathF.Min(a.Min.Z, b.Min.Z)),
        new Vector3(MathF.Max(a.Max.X, b.Max.X), MathF.Max(a.Max.Y, b.Max.Y), MathF.Max(a.Max.Z, b.Max.Z))
    );
  }
}
