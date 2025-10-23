using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.IO;
using Monolith.core;

namespace Monolith
{
  public class Game : GameWindow
  {
    private List<Mesh> scene = new List<Mesh>();
    private List<Texture> textures = new List<Texture>();
    private Camera camera;
    private Shader shader;
    private float time = 0f; // Track elapsed time for rotation
    // private Texture? groundTexture, crateTexture, monolithTexture;
    
    public Game(GameWindowSettings gs, NativeWindowSettings ns)
    : base(gs, ns)
    {
      float[] vertices;
      uint[] indices;
      (vertices, indices) = Geometry.BuildPlane();
      scene.Add(new Mesh(MeshType.Plane, vertices, indices));

      (vertices, indices) = Geometry.BuildCube();
      scene.Add(new Mesh(MeshType.Cube, vertices, indices));
      scene.Add(new Mesh(MeshType.Cube, vertices, indices));

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
        textures.Add(new Texture(groundTexturePath));

      if (File.Exists(crateTexturePath))
        textures.Add(new Texture(crateTexturePath));

      if (File.Exists(monolithTexturePath))
        textures.Add(new Texture(monolithTexturePath));
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

      // Ground
      shader.SetBool("uUseTexture", true);
      shader.SetInt("uTex", 0);
      textures[0]?.Use(TextureUnit.Texture0);
      shader.SetMatrix4("uModel", new Transform().Model);
      scene[0].Draw();

      // Crate
      textures[1]?.Use(TextureUnit.Texture0);
      Transform transform = new Transform();
      transform.Scale = new Vector3(3, 3, 3);
      transform.Position = new Vector3(3, 1.5f, -5);
      shader.SetMatrix4("uModel", transform.Model);
      scene[1].Draw();

      // Monolith
      textures[2]?.Use(TextureUnit.Texture0);
      transform = new Transform();
      transform.Position = new Vector3(-3, 3, -5);
      // Slow rotation on multiple axes (0.3 radians/sec on each axis)
      float rotX = time * 0.3f; // Slow rotation around X-axis
      float rotY = time * 0.5f; // Slightly faster on Y-axis
      float rotZ = time * 0.2f; // Even slower on Z-axis
      transform.Rotation = new Vector3(rotX, rotY, rotZ);
      shader.SetMatrix4("uModel", transform.Model);
      scene[2].Draw();



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
      foreach (var item in scene) { item.Dispose(); }
      foreach (var item in textures) { item.Dispose(); }
      shader.Dispose();
    }
  }
}