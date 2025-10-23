using OpenTK.Mathematics;

namespace Monolith.core
{
  public class Camera
  {
    public Vector3 Position;
    public float yaw, pitch;
    public float fov = 45f;
    public float aspect = 16f / 9f;

    public float sensitivity = 0.2f;
    public float velocity = 8f;
    public float Near = 0.1f; // near and far can be passed in perspective fov, determine distance of render
    public float Far = 200f;

    public Vector3 Forward { get; private set; } = -Vector3.UnitZ;
    public Vector3 Right { get; private set; } = Vector3.UnitX;
    public Vector3 Up { get; private set; } = Vector3.UnitY;

    public Camera(Vector3 pos)
    {
      this.Position = pos;
      this.yaw = -90f;
      this.pitch = 0f;
      RecomputeBasis();
    }

    public Matrix4 View =>
      Matrix4.LookAt(Position, Position + Forward, Up);
    

    public Matrix4 Projection =>
      Matrix4.CreatePerspectiveFieldOfView(
        MathHelper.DegreesToRadians(fov),
        aspect,
        Near, 
        Far
      );

    public void ProcessMouseDelta(float dx, float dy)
    {
      yaw += dx * sensitivity;
      float pitchDelta = dy * sensitivity;
      if (pitch - pitchDelta < 90f && pitch - pitchDelta > -90f)
      {
        pitch -= pitchDelta;
      }
      RecomputeBasis();
    }

    public void MoveLocal(Vector3 dir, float deltaTime)
    {
      var wish = dir;
      if (wish.LengthSquared > 1e-6f) wish = wish.Normalized(); // normalize the diagonal if norm is not too small

      var flatFwd = Forward;
      flatFwd.Y = 0f;

      var flatRight = Right;
      flatRight.Y = 0f;

      if (flatFwd.LengthSquared > 1e-6f) flatFwd = flatFwd.Normalized(); 
      if (flatRight.LengthSquared > 1e-6f) flatRight = flatRight.Normalized();

      var move = flatRight * wish.X + Vector3.UnitY * wish.Y + flatFwd * wish.Z;
      Position += move * velocity * deltaTime;
    }

    public void SetAspect(int width, int height)
    {
      aspect = (height > 0) ? (float)width / height : aspect;
    }

    public void AdjustFov(float delta)
    {
      fov = MathHelper.Clamp(fov + delta, 20f, 90f);
    }

    private void RecomputeBasis()
    {
      float yawRad = MathHelper.DegreesToRadians(yaw);
      float pitchRad = MathHelper.DegreesToRadians(pitch);

      var f = new Vector3(
        MathF.Cos(yawRad) * MathF.Cos(pitchRad),
        MathF.Sin(pitchRad),
        MathF.Sin(yawRad) * MathF.Cos(pitchRad)
      );

      Forward = f.Normalized();
      Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
      Up = Vector3.Normalize(Vector3.Cross(Right, Forward));
    }
  }
}