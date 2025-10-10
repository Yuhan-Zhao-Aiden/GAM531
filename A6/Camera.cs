using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PhongLighting
{
  public class Camera
  {
    public Vector3 Position { get; set; } = new(0f, 0f, 5f);
    private float yaw = -90f; 
    private float pitch = 0f; 

    public float MovementSpeed { get; set; } = 3f;
    public float MouseSensitivity { get; set; } = 0.1f;

    private float fov = 60f;
    public float ZoomSensitivity { get; set; } = 2f;
    private const float MinFov = 30f;
    private const float MaxFov = 90f;

    private Vector2 lastMousePos;
    private bool firstMouseMove = true;

    public float Yaw => yaw;
    public float Pitch => pitch;
    public float Fov => fov;

    public Vector3 Front { get; private set; }
    public Vector3 Right { get; private set; }
    public Vector3 Up { get; private set; } = Vector3.UnitY;

    public Camera()
    {
      UpdateCameraVectors();
    }

    public Camera(Vector3 position, float yaw = -90f, float pitch = 0f) : this()
    {
      Position = position;
      this.yaw = yaw;
      this.pitch = pitch;
      UpdateCameraVectors();
    }

    public void ProcessMovement(KeyboardState keyboardState, float deltaTime)
    {
      float velocity = MovementSpeed * deltaTime;

      if (keyboardState.IsKeyDown(Keys.W))
        Position += Front * velocity;
      if (keyboardState.IsKeyDown(Keys.S))
        Position -= Front * velocity;
      if (keyboardState.IsKeyDown(Keys.A))
        Position -= Right * velocity;
      if (keyboardState.IsKeyDown(Keys.D))
        Position += Right * velocity;
    }

    public void ProcessMouseMovement(float mouseX, float mouseY)
    {
      if (firstMouseMove)
      {
        lastMousePos = new Vector2(mouseX, mouseY);
        firstMouseMove = false;
        return;
      }

      float deltaX = mouseX - lastMousePos.X;
      float deltaY = lastMousePos.Y - mouseY; 

      lastMousePos = new Vector2(mouseX, mouseY);

      deltaX *= MouseSensitivity;
      deltaY *= MouseSensitivity;

      yaw += deltaX;
      pitch += deltaY;

      if (pitch > 90.0f)
        pitch = 90.0f;
      if (pitch < -90.0f)
        pitch = -90.0f;

      UpdateCameraVectors();
    }

    public void ProcessMouseScroll(float scrollY)
    {
      fov -= scrollY * ZoomSensitivity;

      if (fov < MinFov)
        fov = MinFov;
      if (fov > MaxFov)
        fov = MaxFov;
    }

    private void UpdateCameraVectors()
    {
      // The direction of the camera
      Front = new Vector3(
          MathF.Cos(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch)),
          MathF.Sin(MathHelper.DegreesToRadians(pitch)),
          MathF.Sin(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch))
      );
      // So that the length is 1
      Front = Vector3.Normalize(Front);

      // Calculate right and up direction of camera, so that WASD can move cam in correct direction
      Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
      Up = Vector3.Normalize(Vector3.Cross(Right, Front));
    }


    public Matrix4 GetViewMatrix()
    {
      return Matrix4.LookAt(Position, Position + Front, Up);
    }

    public Matrix4 GetProjectionMatrix(float aspectRatio)
    {
      return Matrix4.CreatePerspectiveFieldOfView(
          MathHelper.DegreesToRadians(fov),
          aspectRatio,
          0.1f, 100f
      );
    }

    public Matrix4 GetProjectionMatrix(float width, float height)
    {
      return GetProjectionMatrix(width / height);
    }
  }
}