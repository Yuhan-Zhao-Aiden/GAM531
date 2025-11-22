namespace A9.Core;

using OpenTK.Mathematics;

public class Transform
{
  public Vector3 Position = Vector3.Zero;
  public Vector3 Rotation = Vector3.Zero;
  public Vector3 Scale = Vector3.One;

  public Transform() {}
  public Transform(Vector3 pos, Vector3 rot, Vector3 scale)
  {
    Position = pos;
    Rotation = rot;
    Scale = scale;
  }

  public Matrix4 Model =>
    Matrix4.CreateScale(Scale) *
    Matrix4.CreateRotationX(Rotation.X) *
    Matrix4.CreateRotationY(Rotation.Y) *
    Matrix4.CreateRotationZ(Rotation.Z) *
    Matrix4.CreateTranslation(Position);
}