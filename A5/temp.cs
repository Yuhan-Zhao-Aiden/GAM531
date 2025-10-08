using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace PhongStep1
{
  public class Game : GameWindow
  {
    // GL objects
    private int _vao, _vbo, _ebo, _program;

    // Matrices / camera
    private Matrix4 _model, _view, _projection;
    private float _angle;

    // Uniform locations
    private int _uModel, _uView, _uProj;

    public Game()
      : base(
          new GameWindowSettings { RenderFrequency = 60, UpdateFrequency = 60 },
          new NativeWindowSettings {
            Title = "Step 1 — Cube + Camera + Normals",
            Size = new Vector2i(960, 600),
            APIVersion = new Version(3, 3),
            Profile = ContextProfile.Core
          })
    { }

    protected override void OnLoad()
    {
      base.OnLoad();

      GL.ClearColor(0.07f, 0.07f, 0.1f, 1f);
      GL.Enable(EnableCap.DepthTest);

      // --- Cube geometry (24 unique verts: per-face normals) ---
      // Layout per vertex: position.xyz, normal.xyz  (stride = 6 floats)
      float[] vertices =
      {
        // FRONT (+Z, normal 0,0,1)
        -0.5f, -0.5f, +0.5f,   0f, 0f, 1f,
         0.5f, -0.5f, +0.5f,   0f, 0f, 1f,
         0.5f,  0.5f, +0.5f,   0f, 0f, 1f,
        -0.5f,  0.5f, +0.5f,   0f, 0f, 1f,

        // BACK (-Z, normal 0,0,-1)
        -0.5f, -0.5f, -0.5f,   0f, 0f,-1f,
        -0.5f,  0.5f, -0.5f,   0f, 0f,-1f,
         0.5f,  0.5f, -0.5f,   0f, 0f,-1f,
         0.5f, -0.5f, -0.5f,   0f, 0f,-1f,

        // LEFT (-X, normal -1,0,0)
        -0.5f, -0.5f, -0.5f,  -1f, 0f, 0f,
        -0.5f, -0.5f,  0.5f,  -1f, 0f, 0f,
        -0.5f,  0.5f,  0.5f,  -1f, 0f, 0f,
        -0.5f,  0.5f, -0.5f,  -1f, 0f, 0f,

        // RIGHT (+X, normal +1,0,0)
         0.5f, -0.5f, -0.5f,   1f, 0f, 0f,
         0.5f,  0.5f, -0.5f,   1f, 0f, 0f,
         0.5f,  0.5f,  0.5f,   1f, 0f, 0f,
         0.5f, -0.5f,  0.5f,   1f, 0f, 0f,

        // TOP (+Y, normal 0,+1,0)
        -0.5f,  0.5f, -0.5f,   0f, 1f, 0f,
        -0.5f,  0.5f,  0.5f,   0f, 1f, 0f,
         0.5f,  0.5f,  0.5f,   0f, 1f, 0f,
         0.5f,  0.5f, -0.5f,   0f, 1f, 0f,

        // BOTTOM (-Y, normal 0,-1,0)
        -0.5f, -0.5f, -0.5f,   0f,-1f, 0f,
         0.5f, -0.5f, -0.5f,   0f,-1f, 0f,
         0.5f, -0.5f,  0.5f,   0f,-1f, 0f,
        -0.5f, -0.5f,  0.5f,   0f,-1f, 0f,
      };

      uint[] indices =
      {
        // front
        0,1,2,  2,3,0,
        // back
        4,5,6,  6,7,4,
        // left
        8,9,10, 10,11,8,
        // right
        12,13,14, 14,15,12,
        // top
        16,17,18, 18,19,16,
        // bottom
        20,21,22, 22,23,20
      };

      _vao = GL.GenVertexArray();
      _vbo = GL.GenBuffer();
      _ebo = GL.GenBuffer();

      GL.BindVertexArray(_vao);

      GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
      GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
      GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

      // position (location=0)
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);

      // normal (location=1)
      GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
      GL.EnableVertexAttribArray(1);

      GL.BindVertexArray(0);

      // Compile minimal shaders that visualize normals (for debugging)
      _program = CreateProgram(_vsSource, _fsNormalsDebug);
      GL.UseProgram(_program);

      _uModel = GL.GetUniformLocation(_program, "model");
      _uView  = GL.GetUniformLocation(_program, "view");
      _uProj  = GL.GetUniformLocation(_program, "projection");

      // Camera: place at an angle and look at origin
      var eye = new Vector3(3f, 2f, 4f);
      var target = Vector3.Zero;
      var up = Vector3.UnitY;

      _view = Matrix4.LookAt(eye, target, up);
      _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f),
      Size.X / (float)Size.Y,
      0.1f, 100f);

      CursorGrabbed = false; // free mouse for now
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);
      GL.Viewport(0, 0, Size.X, Size.Y);
      _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f),
        Size.X / (float)Size.Y,
        0.1f, 100f);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);
      _angle += (float)args.Time * 0.7f; // gentle spin to show faces/normals
      _model = Matrix4.CreateRotationY(_angle) * Matrix4.CreateRotationX(_angle * 0.5f);

      if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
        Close();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      GL.UseProgram(_program);
      GL.UniformMatrix4(_uModel, false, ref _model);
      GL.UniformMatrix4(_uView,  false, ref _view);
      GL.UniformMatrix4(_uProj,  false, ref _projection);

      GL.BindVertexArray(_vao);
      GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);
      GL.BindVertexArray(0);

      SwapBuffers();
    }

    protected override void OnUnload()
    {
      base.OnUnload();
      GL.DeleteBuffer(_ebo);
      GL.DeleteBuffer(_vbo);
      GL.DeleteVertexArray(_vao);
      GL.DeleteProgram(_program);
    }

    // --- shader helpers ---
    private static int CreateShader(ShaderType type, string src)
    {
      int s = GL.CreateShader(type);
      GL.ShaderSource(s, src);
      GL.CompileShader(s);
      GL.GetShader(s, ShaderParameter.CompileStatus, out int ok);
      if (ok == 0)
      {
        string log = GL.GetShaderInfoLog(s);
        throw new Exception($"Shader compile error ({type}):\n{log}");
      }
      return s;
    }

    private static int CreateProgram(string vs, string fs)
    {
      int v = CreateShader(ShaderType.VertexShader, vs);
      int f = CreateShader(ShaderType.FragmentShader, fs);
      int p = GL.CreateProgram();
      GL.AttachShader(p, v);
      GL.AttachShader(p, f);
      GL.LinkProgram(p);
      GL.GetProgram(p, GetProgramParameterName.LinkStatus, out int ok);
      GL.DetachShader(p, v);
      GL.DetachShader(p, f);
      GL.DeleteShader(v);
      GL.DeleteShader(f);
      if (ok == 0)
      {
        string log = GL.GetProgramInfoLog(p);
        throw new Exception($"Program link error:\n{log}");
      }
      return p;
    }

    // --- GLSL sources: normal-visualization (replace FS later with Phong) ---
    private const string _vsSource = @"
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

    // Proper normal transform
    mat3 normalMatrix = mat3(transpose(inverse(model)));
    Normal = normalize(normalMatrix * aNormal);

    gl_Position = projection * view * worldPos;
}
";

    // Debug FS: visualize normals (mapped from [-1,1] to [0,1])
    private const string _fsNormalsDebug = @"
#version 330 core
in vec3 Normal;
out vec4 FragColor;
void main()
{
    vec3 n = normalize(Normal);
    FragColor = vec4(0.5*n + 0.5, 1.0);
}
";
  }
}
