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
    private Camera camera;
    private Shader shader;
    private float time = 0f; // Track elapsed time for rotation
    private SceneObject? monolithObject;
    // private Texture? groundTexture, crateTexture, monolithTexture;
    
    public Game(GameWindowSettings gs, NativeWindowSettings ns)
    : base(gs, ns)
    {
      float[] vertices;
      uint[] indices;
      (vertices, indices) = Geometry.BuildPlane();
      var planeMesh = new Mesh(MeshType.Plane, vertices, indices);
      var groundObject = new SceneObject(planeMesh, null);
      sceneObjects.Add(groundObject);

      (vertices, indices) = Geometry.BuildCube();
      var crateMesh = new Mesh(MeshType.Cube, vertices, indices);
      var crateTransform = new Transform
      {
        Scale = new Vector3(3, 3, 3),
        Position = new Vector3(3, 1.5f, -5)
      };
      var crateObject = new SceneObject(crateMesh, null, crateTransform);
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
      string groundTexturePath = Path.Combine(baseDir, "Assets", "StoneBricks.jpg");
      string crateTexturePath = Path.Combine(baseDir, "Assets", "crate.png");
      string monolithTexturePath = Path.Combine(baseDir, "Assets", "Monolith.jpg");
      if (File.Exists(groundTexturePath))
        groundObject.Texture = new Texture(groundTexturePath);

      if (File.Exists(crateTexturePath))
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
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      base.OnMouseMove(e);
      camera.ProcessMouseDelta(e.Delta.X, e.Delta.Y);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);

      // Update time for animation
      time += (float)args.Time;

      var kb = KeyboardState;
      var move = Vector3.Zero;
      if (kb.IsKeyDown(Keys.W)) move.Z += 1;
      if (kb.IsKeyDown(Keys.S)) move.Z -= 1;
      if (kb.IsKeyDown(Keys.A)) move.X -= 1;
      if (kb.IsKeyDown(Keys.D)) move.X += 1;

      camera.MoveLocal(move, (float)args.Time);

      if (kb.IsKeyDown(Keys.Escape)) CursorState = CursorState.Normal;
      if (MouseState.IsButtonDown(MouseButton.Left) && CursorState == CursorState.Normal)
        CursorState = CursorState.Grabbed;
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
      shader.Dispose();
    }
  }
}
