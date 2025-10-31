using System.Collections.Generic;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.IO;
using Monolith.core;
using Monolith.Physics;

namespace Monolith
{
  public class Game : GameWindow
  {
    private readonly List<SceneObject> sceneObjects = new();
    private readonly List<PhysicsBody> dynamicBodies = new();
    private readonly List<SceneObject> projectiles = new();
    private readonly PhysicsWorld physicsWorld = new();
    private double physicsAccumulator = 0f;
    private PhysicsBody? cameraBody;
    private PhysicsBody? groundBody;
    private Camera camera;
    private Shader shader;
    private float time = 0f; // Track elapsed time for rotation
    private SceneObject? groundObject;
    private SceneObject? crateObject;
    private SceneObject? monolithObject;
    private Texture? bulletTexture;
    private double shootCooldown = 0d;
    private const double ShootInterval = 0.25d;
    
    public Game(GameWindowSettings gs, NativeWindowSettings ns)
    : base(gs, ns)
    {
      VSync = VSyncMode.On;
      
      float[] vertices;
      uint[] indices;
      (vertices, indices) = Geometry.BuildPlane();
      var planeMesh = new Mesh(MeshType.Plane, vertices, indices);
      groundObject = new SceneObject(planeMesh, null);
      sceneObjects.Add(groundObject);

      (vertices, indices) = Geometry.BuildCube();
      var crateMesh = new Mesh(MeshType.Cube, vertices, indices);
      var crateTransform = new Transform
      {
        Scale = new Vector3(3, 3, 3),
        Position = new Vector3(3, 1.5f, -5)
      };
      crateObject = new SceneObject(crateMesh, null, crateTransform);
      sceneObjects.Add(crateObject);

      var monolithMesh = new Mesh(MeshType.Cube, vertices, indices);
      var monolithTransform = new Transform
      {
        Position = new Vector3(-3, 3, -5)
      };
      monolithObject = new SceneObject(monolithMesh, null, monolithTransform);
      sceneObjects.Add(monolithObject);

      // Camera
      camera = new Camera(new Vector3(0, 3, 3));

      // Shader
      string baseDir = AppContext.BaseDirectory;
      string vsPath = Path.Combine(baseDir, "Shaders", "vertex.glsl");
      string fsPath = Path.Combine(baseDir, "Shaders", "fragment.glsl");
      shader = new Shader(vsPath, fsPath);

      // Texture
      string bulletTexturePath = Path.Combine(baseDir, "Assets", "Bullet.jpg");
      if (File.Exists(bulletTexturePath))
        bulletTexture = new Texture(bulletTexturePath);

      string groundTexturePath = Path.Combine(baseDir, "Assets", "StoneBricks.jpg");
      string crateTexturePath = Path.Combine(baseDir, "Assets", "crate.png");
      string monolithTexturePath = Path.Combine(baseDir, "Assets", "Monolith.jpg");
      if (File.Exists(groundTexturePath) && groundObject != null)
        groundObject.Texture = new Texture(groundTexturePath);

      if (File.Exists(crateTexturePath) && crateObject != null)
        crateObject.Texture = new Texture(crateTexturePath);

      if (File.Exists(monolithTexturePath) && monolithObject != null)
        monolithObject.Texture = new Texture(monolithTexturePath);
    }

    protected override void OnLoad()
    {
      base.OnLoad();
      GL.ClearColor(0.07f, 0.07f, 0.07f, 1f);
      GL.Enable(EnableCap.DepthTest);

      CursorState = CursorState.Grabbed;

      if (groundObject != null)
      {
        groundBody = PhysicsBody.CreateStaticBox(new Vector3(0f, -0.05f, 0f), new Vector3(25f, 0.05f, 25f));
        groundObject.Body = groundBody;
        physicsWorld.Bodies.Add(groundBody);
      }

      if (crateObject != null)
      {
        var crateBody = PhysicsBody.CreateDynamicBox(crateObject.Transform.Position, new Vector3(1.5f));
        crateBody.Mass = 5f;
        crateObject.Body = crateBody;
        physicsWorld.Bodies.Add(crateBody);
        dynamicBodies.Add(crateBody);
      }

      if (monolithObject != null)
      {
        var monolithBody = PhysicsBody.CreateStaticBox(monolithObject.Transform.Position, new Vector3(0.5f));
        monolithObject.Body = monolithBody;
        physicsWorld.Bodies.Add(monolithBody);
      }

      cameraBody = PhysicsBody.CreateDynamicBox(camera.Position, new Vector3(0.4f, 0.9f, 0.4f), restitution: 0f);
      cameraBody.Mass = 1f;
      physicsWorld.Bodies.Add(cameraBody);
      dynamicBodies.Add(cameraBody);
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      base.OnMouseMove(e);
      camera.ProcessMouseDelta(e.Delta.X, e.Delta.Y);
    }

    protected void Shoot()
    {
      if (projectiles.Count >= 10)
      {
        projectiles[0].Destroy(dynamicBodies, physicsWorld.Bodies, sceneObjects, disposeTexture: false);
        projectiles.RemoveAt(0);
      }

      var forward = camera.Forward;
      if (forward.LengthSquared < 1e-6f) return;
      forward = forward.Normalized();

      Vector3 spawnOffset = forward * 1.0f;
      if (cameraBody != null)
        spawnOffset += Vector3.UnitY * (cameraBody.HalfExtents.Y * 0.5f);

      Vector3 spawnPosition = camera.Position + spawnOffset;

      var projectileBody = PhysicsBody.CreateDynamicBox(spawnPosition, new Vector3(0.25f, 0.25f, 0.25f), restitution: 0.2f);
      projectileBody.Mass = 1f;
      projectileBody.Velocity = forward * 25f + new Vector3(0f, cameraBody?.Velocity.Y ?? 0f, 0f);

      var (vertices, indices) = Geometry.BuildCube();
      var projectileTransform = new Transform
      {
        Position = spawnPosition,
        Scale = new Vector3(0.5f)
      };

      var newProjectile = new SceneObject(
        new Mesh(MeshType.Cube, vertices, indices),
        texture: bulletTexture,
        transform: projectileTransform,
        body: projectileBody
      );

      projectiles.Add(newProjectile);
      sceneObjects.Add(newProjectile);
      physicsWorld.Bodies.Add(projectileBody);
      dynamicBodies.Add(projectileBody);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);

      // Update time for animation
      time += (float)args.Time;
      shootCooldown = System.Math.Max(0d, shootCooldown - args.Time);

      var kb = KeyboardState;
      var move = Vector3.Zero;
      if (kb.IsKeyDown(Keys.W)) move.Z += 1;
      if (kb.IsKeyDown(Keys.S)) move.Z -= 1;
      if (kb.IsKeyDown(Keys.A)) move.X -= 1;
      if (kb.IsKeyDown(Keys.D)) move.X += 1;

      camera.MoveLocal(move, (float)args.Time);

      if (cameraBody != null)
      {
        cameraBody.Position.X = camera.Position.X;
        cameraBody.Position.Z = camera.Position.Z;
      }

      if (kb.IsKeyPressed(Keys.Space) && cameraBody != null)
      {
        bool grounded = false;
        if (groundBody != null)
        {
          float groundSurface = groundBody.Max.Y;
          grounded = cameraBody.Min.Y <= groundSurface + 0.05f;
        }

        if (grounded && cameraBody.Velocity.Y <= 0f)
        {
          cameraBody.Velocity.Y = 6f;
        }
      }

      physicsAccumulator += args.Time;
      while (physicsAccumulator >= PhysicsWorld.FixedDt)
      {
        physicsWorld.Step(PhysicsWorld.FixedDt);
        physicsAccumulator -= PhysicsWorld.FixedDt;
      }

      foreach (var obj in sceneObjects)
      {
        if (obj.Body != null && !obj.Body.IsStatic)
        {
          obj.Transform.Position = obj.Body.Position;
        }
      }

      if (cameraBody != null)
      {
        var camPos = camera.Position;
        camPos.Y = cameraBody.Position.Y;
        camera.Position = camPos;

        cameraBody.Position.X = camera.Position.X;
        cameraBody.Position.Z = camera.Position.Z;
      }

      if (kb.IsKeyDown(Keys.Escape)) CursorState = CursorState.Normal;
      if (MouseState.IsButtonDown(MouseButton.Left) && CursorState == CursorState.Normal)
        CursorState = CursorState.Grabbed;

      if (MouseState.IsButtonDown(MouseButton.Left) && shootCooldown <= 0d)
      {
        Shoot();
        shootCooldown = ShootInterval;
      }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      shader.Use();
      shader.SetMatrix4("uView", camera.View);
      shader.SetMatrix4("uProj", camera.Projection);
      shader.SetVector3("uLightPos", new Vector3(0, 6, 5));
      shader.SetVector3("uLightColor", new Vector3(1f, 1f, 1f));
      shader.SetVector3("uViewPos", camera.Position);

      if (monolithObject != null)
      {
        // Slow rotation on multiple axes (0.3 radians/sec on each axis)
        float rotX = time * 0.3f; // Slow rotation around X-axis
        float rotY = time * 0.5f; // Slightly faster on Y-axis
        float rotZ = time * 0.2f; // Even slower on Z-axis
        monolithObject.Transform.Rotation = new Vector3(rotX, rotY, rotZ);
      }

      foreach (var obj in sceneObjects)
      {
        bool hasTexture = obj.Texture != null;
        shader.SetBool("uUseTexture", hasTexture);
        if (hasTexture && obj.Texture != null)
        {
          shader.SetInt("uTex", 0);
          obj.Texture.Use(TextureUnit.Texture0);
        }
        shader.SetMatrix4("uModel", obj.Transform.Model);
        obj.Mesh.Draw();
      }


      SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
      base.OnFramebufferResize(e);
      GL.Viewport(0, 0, e.Width, e.Height);
      camera.SetAspect(e.Width, e.Height);
    }

    protected override void OnUnload()
    {
      base.OnUnload();
      var disposedTextures = new HashSet<Texture>();
      foreach (var obj in sceneObjects)
      {
        obj.Mesh.Dispose();
        if (obj.Texture != null && disposedTextures.Add(obj.Texture))
        {
          obj.Texture.Dispose();
        }
      }
      if (bulletTexture != null)
      {
        bulletTexture.Dispose();
        bulletTexture = null;
      }
      shader.Dispose();
    }
  }
}
