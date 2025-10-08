using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.IO.Compression;
using OpenTK.Compute.OpenCL;

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

    //GLSL
    private const string vShader = @"
    #version 330 core
    layout (location=0) in vec3 aPosition;
    layout (location=1) in vec3 aNormal;

    out vec3 FragPos;
    out vec3 Normal;

    uniform mat4
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

    }

    private static int CreateShader(ShaderType type, string src) {
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
  }
}