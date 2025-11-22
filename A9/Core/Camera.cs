using OpenTK.Mathematics;

namespace A9.Core;

public class Camera
{
  public Vector3 Position { get; set; }
  public Vector3 Target { get; set; }
  public Vector3 Up { get; set; } = Vector3.UnitY;

  public float FieldOfView { get; set; } = 45.0f;
  public float AspectRatio { get; set; }
  public float NearPlane { get; set; } = 0.1f;
  public float FarPlane { get; set; } = 100.0f;

  // Third-person camera settings
  public GameObject? FollowTarget { get; set; }
  public float Distance { get; set; } = 8.0f;
  public float Height { get; set; } = 1.5f;
  public float MouseSensitivity { get; set; } = 0.002f;
  public float PitchSensitivity { get; set; } = 0.001f; // Slower vertical movement

  // Camera rotation
  private float _yaw = 0.0f;
  private float _pitch = 0.3f;
  private const float MaxPitch = 1.4f;
  private const float MinPitch = -0.5f;

  public Camera(Vector3 position, Vector3 target, float aspectRatio)
  {
    Position = position;
    Target = target;
    AspectRatio = aspectRatio;
  }

  public void ProcessMouseMovement(float deltaX, float deltaY)
  {
    _yaw -= deltaX * MouseSensitivity;
    _pitch += deltaY * PitchSensitivity;

    _pitch = MathHelper.Clamp(_pitch, MinPitch, MaxPitch);
  }

  public Matrix4 GetViewMatrix()
  {
    return Matrix4.LookAt(Position, Target, Up);
  }

  public Matrix4 GetProjectionMatrix()
  {
    return Matrix4.CreatePerspectiveFieldOfView(
        MathHelper.DegreesToRadians(FieldOfView),
        AspectRatio,
        NearPlane,
        FarPlane
    );
  }

  public void Update(float deltaTime)
  {
    if (FollowTarget != null)
    {
      float horizontalDistance = Distance * (float)Math.Cos(_pitch);
      float verticalDistance = Distance * (float)Math.Sin(_pitch);

      float offsetX = horizontalDistance * (float)Math.Sin(_yaw);
      float offsetZ = horizontalDistance * (float)Math.Cos(_yaw);

      Vector3 targetPos = FollowTarget.Transform.Position + new Vector3(0, Height, 0);
      Position = targetPos + new Vector3(offsetX, verticalDistance, offsetZ);
      Target = targetPos;
    }
  }

  public void UpdateAspectRatio(float aspectRatio)
  {
    AspectRatio = aspectRatio;
  }

  public float GetYaw() => _yaw;
}
