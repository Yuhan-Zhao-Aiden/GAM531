using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PhongLighting
{
  public class Game : GameWindow
  {
    private int vao, vbo, ebo, program;
    private float[] vertices;
    private uint[] indices;
    private Matrix4 model, view, proj;
    private float angle;
    private int uModel, uView, uProj;
    private Vector3 eye = new(0f, 0f, 5f);

    // Phong lighting
    private Vector3 lightPos = new(2f, 2f, 2f);
    private Vector3 lightColor = Vector3.One; // (1, 1, 1)
    private Vector3 objectColor = new(1.0f, 0.85f, 0.7f);

    private float yaw = -90f;  // control via Q and E
    private float moveSpeed = 3f;

    private int uLightPos, uViewPos, uLightColor, uObjectColor;

    //GLSL
    private const string vShader = @"
    #version 330 core
    layout (location=0) in vec3 aPosition;
    layout (location=1) in vec3 aNormal;

    out vec3 FragPos;
    out vec3 Normal;

    uniform mat4 model;
    uniform mat4 view;
    uniform mat4 projection;

    void main()
    {
      vec4 worldPos = model * vec4(aPosition, 1.0);
      FragPos = worldPos.xyz;
      mat3 normalMatrix = mat3(transpose(inverse(model)));
      Normal = normalize(normalMatrix * aNormal);

      gl_Position = projection * view * worldPos;
    }
    ";

    private const string fShader = @"
    #version 330 core
    out vec4 FragColor;

    in vec3 FragPos;
    in vec3 Normal;

    uniform vec3 lightPos;
    uniform vec3 viewPos;
    uniform vec3 lightColor;
    uniform vec3 objectColor;

    void main()
    {
      vec3 ambient = 0.12f * lightColor; // ambient

      vec3 n = normalize(Normal); // diffused
      vec3 L = normalize(lightPos - FragPos);
      float diff = max(dot(n, L), 0.0);
      vec3 diffused = diff * lightColor;

      vec3 V = normalize(viewPos - FragPos);
      vec3 R = reflect(-L, n);
      float spec = pow(max(dot(V, R), 0.0), 50f);
      vec3 specular = 0.5f * spec * lightColor;


      vec3 color = (ambient + diffused + specular) * objectColor;
      FragColor = vec4(color, 1.0);
    }
    ";

    public Game(GameWindowSettings gs, NativeWindowSettings ns) : base(gs, ns)
    {
      (vertices, indices) = Utility.BuildCube();
    }

    protected override void OnLoad()
    {
      base.OnLoad();
      GL.ClearColor(0.07f, 0.07f, 0.07f, 1f);
      GL.Enable(EnableCap.DepthTest);

      vao = GL.GenVertexArray();
      vbo = GL.GenBuffer();
      ebo = GL.GenBuffer();

      GL.BindVertexArray(vao);
      GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
      GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

      GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
      GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);

      GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
      GL.EnableVertexAttribArray(1);

      GL.BindVertexArray(0);

      program = CreateProgram(vShader, fShader);
      GL.UseProgram(program);
      uModel = GL.GetUniformLocation(program, "model");
      uView = GL.GetUniformLocation(program, "view");
      uProj = GL.GetUniformLocation(program, "projection");

      uLightPos = GL.GetUniformLocation(program, "lightPos");
      uViewPos = GL.GetUniformLocation(program, "viewPos");
      uLightColor = GL.GetUniformLocation(program, "lightColor");
      uObjectColor = GL.GetUniformLocation(program, "objectColor");



      view = Matrix4.LookAt(eye, Vector3.Zero, Vector3.UnitY);
      proj = Matrix4.CreatePerspectiveFieldOfView(
        MathHelper.DegreesToRadians(60f),
        Size.X / (float)Size.Y,
        0.1f, 100f
      );

      // model = Matrix4.CreateRotationX(angle);
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
      base.OnFramebufferResize(e);
      GL.Viewport(0, 0, e.Width, e.Height);
      proj = Matrix4.CreatePerspectiveFieldOfView(
        MathHelper.DegreesToRadians(60f),
        e.Width / (float)e.Height,
        0.1f, 100f
      );
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);
      angle += (float)args.Time * 0.7f;
      model = Matrix4.CreateRotationY(angle) * Matrix4.CreateRotationX(angle * 0.5f);

      // We need to calculate to calculate the forward direction 
      // Because we can change yaw, if you rotate the camear, WS will move along
      // the camear direction
      Vector3 front = new Vector3(
            MathF.Cos(MathHelper.DegreesToRadians(yaw)),
            0f,
            MathF.Sin(MathHelper.DegreesToRadians(yaw))
        );
      front = Vector3.Normalize(front);

      Vector3 right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));

      float deltaTime = (float)args.Time;
      float velocity = moveSpeed * deltaTime;

      // Camera control, WASD to move, QE to increase and decrease yaw
      if (KeyboardState.IsKeyDown(Keys.W))
        eye += front * velocity;
      if (KeyboardState.IsKeyDown(Keys.S))
        eye -= front * velocity;
      if (KeyboardState.IsKeyDown(Keys.A))
        eye -= right * velocity;
      if (KeyboardState.IsKeyDown(Keys.D))
        eye += right * velocity;

      if (KeyboardState.IsKeyDown(Keys.Q))
        yaw -= 90f * deltaTime;
      if (KeyboardState.IsKeyDown(Keys.E))
        yaw += 90f * deltaTime;

    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


      Vector3 front = new Vector3(
            MathF.Cos(MathHelper.DegreesToRadians(yaw)),
            0f,
            MathF.Sin(MathHelper.DegreesToRadians(yaw))
        );
      Vector3 target = eye + Vector3.Normalize(front);
      view = Matrix4.LookAt(eye, target, Vector3.UnitY);

      GL.UseProgram(program);
      GL.UniformMatrix4(uModel, false, ref model);
      GL.UniformMatrix4(uView, false, ref view);
      GL.UniformMatrix4(uProj, false, ref proj);

      // lighting params
      GL.Uniform3(uLightPos, lightPos);
      GL.Uniform3(uViewPos, eye);
      GL.Uniform3(uLightColor, lightColor);
      GL.Uniform3(uObjectColor, objectColor);


      GL.BindVertexArray(vao);
      GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);
      GL.BindVertexArray(0);
      SwapBuffers();
    }

    private static int CreateShader(ShaderType type, string src)
    {
      int s = GL.CreateShader(type);
      GL.ShaderSource(s, src);
      GL.CompileShader(s);
      GL.GetShader(s, ShaderParameter.CompileStatus, out int ok);
      if (ok == 0)
      {
        throw new Exception($"Error when compiling shader: {type}");
      }
      return s;
    }

    private static int CreateProgram(string vs, string fs)
    {
      int p = 0;
      try
      {
        int v = CreateShader(ShaderType.VertexShader, vs);
        int f = CreateShader(ShaderType.FragmentShader, fs);
        p = GL.CreateProgram();
        GL.AttachShader(p, v);
        GL.AttachShader(p, f);
        GL.LinkProgram(p);
        GL.GetProgram(p, GetProgramParameterName.LinkStatus, out int ok);
        if (ok == 0) { throw new Exception("Error during linking"); }

        GL.DetachShader(p, v);
        GL.DetachShader(p, f);
        GL.DeleteShader(v);
        GL.DeleteShader(f);
      }
      catch (Exception e) { Console.WriteLine(e); }
      return p;
    }

    protected override void OnUnload()
    {
      base.OnUnload();
      GL.DeleteBuffer(ebo);
      GL.DeleteBuffer(vbo);
      GL.DeleteVertexArray(vao);
      GL.DeleteProgram(program);
    }
  }
}