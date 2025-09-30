using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CubeRender
{
  public class Game : GameWindow
  {
    private int vao, vbo, ebo, program;
    private Matrix4 proj, view, model;
    private float angle = 0f;

    private int uModelLoc, uViewLoc, uProjLoc;

    private readonly float[] vertices =
    {

        -0.5f,-0.5f,-0.5f,1f, 0f, 0f, // first 3 for position, last 3 for color rgb
        0.5f,-0.5f,-0.5f,1f, 0f, 0f,
        0.5f, 0.5f,-0.5f,1f, 0f, 0f,
        -0.5f, 0.5f,-0.5f,1f, 0f, 0f,
        -0.5f,-0.5f, 0.5f,0f, 1f, 0f,
        0.5f,-0.5f, 0.5f,0f, 1f, 0f,
        0.5f, 0.5f, 0.5f,0f, 0f, 1f,
        -0.5f, 0.5f, 0.5f,0f,0f,1f
    }; // these 8 vertices label 0-7, will be used in indices array

    // 12 triangles (two per face)
    private readonly uint[] indices =
    {
        0, 1, 2,  2, 3, 0,
        4, 5, 6,  6, 7, 4,
        4, 0, 3,  3, 7, 4,
        1, 5, 6,  6, 2, 1,
        4, 5, 1,  1, 0, 4,
        3, 2, 6,  6, 7, 3
    };

    //GLSL
    // This program render vertex in correct position and output vec3 color to fragment shader
    private const string VertexSrc = @"
#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColor;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProj;

out vec3 vColor;

void main()
{
    vColor = aColor;
    gl_Position = uProj * uView * uModel * vec4(aPosition, 1.0);
}
";

//output color
    private const string FragmentSrc = @"
#version 330 core
in vec3 vColor;
out vec4 FragColor;

void main()
{
    FragColor = vec4(vColor, 1.0);
}
";

    public Game(GameWindowSettings gs, NativeWindowSettings ns)
    : base(gs, ns) { }

    private static int CreateProgram(string vertSrcCode, string fragSrcCode)
    {
      int v = GL.CreateShader(ShaderType.VertexShader);
      GL.ShaderSource(v, vertSrcCode);
      GL.CompileShader(v);
      GL.GetShader(v, ShaderParameter.CompileStatus, out int vStatus);
      if (vStatus == 0)
        throw new System.Exception("Error when compiling vertex shader");

      int f = GL.CreateShader(ShaderType.FragmentShader);
      GL.ShaderSource(f, fragSrcCode);
      GL.CompileShader(f);
      GL.GetShader(f, ShaderParameter.CompileStatus, out int fStatus);
      if (fStatus == 0)
        throw new System.Exception("Error when compiling fragment shader");

      int p = GL.CreateProgram();
      GL.AttachShader(p, v);
      GL.AttachShader(p, f);
      GL.LinkProgram(p);
      GL.GetProgram(p, GetProgramParameterName.LinkStatus, out int linkStatus);
      if (linkStatus == 0)
        throw new System.Exception("Linker error");

      GL.DetachShader(p, v);
      GL.DetachShader(p, f);
      GL.DeleteShader(v);
      GL.DeleteShader(f);

      return p;
    }

    protected override void OnLoad()
    {
      base.OnLoad();
      GL.ClearColor(Color4.CornflowerBlue);
      GL.Enable(EnableCap.DepthTest);
      program = CreateProgram(VertexSrc, FragmentSrc);
      GL.UseProgram(program);

      vao = GL.GenVertexArray(); // 
      vbo = GL.GenBuffer();
      ebo = GL.GenBuffer();

      GL.BindVertexArray(vao);

      GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
      GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

      GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
      GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

      int stride = 6 * sizeof(float); // This is the data size of 1 vertex
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
      GL.EnableVertexAttribArray(0); // the first 3 consider as one thing xyz

      GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
      GL.EnableVertexAttribArray(1); // the next 3 consider as rgb color

      view = Matrix4.CreateTranslation(1f, 0f, -3f); // Set the view camera..
      RebuildProjection(); 

      uModelLoc = GL.GetUniformLocation(program, "uModel");
      uViewLoc = GL.GetUniformLocation(program, "uView");
      uProjLoc = GL.GetUniformLocation(program, "uProj");

      GL.UniformMatrix4(uViewLoc, false, ref view);
      GL.UniformMatrix4(uProjLoc, false, ref proj);
    }

    private void RebuildProjection()
    {
      float aspect = Size.X / (float)Size.Y;
      proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f),aspect,0.1f,100f);
    }
    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // clear each frame

      // Draw cube
      GL.UseProgram(program);
      GL.BindVertexArray(vao);
      GL.UniformMatrix4(uModelLoc, false, ref model);
      GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);


      SwapBuffers();
    }

    protected override void OnUnload()
    {
      base.OnUnload();

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);

      GL.DeleteBuffer(vbo);
      GL.DeleteBuffer(ebo);
      GL.DeleteVertexArray(vao);
      GL.DeleteProgram(program);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);
      GL.Viewport(0, 0, Size.X, Size.Y);
      RebuildProjection();
      GL.UseProgram(program);
      GL.UniformMatrix4(uProjLoc, false, ref proj);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);
      angle += (float)args.Time * MathHelper.DegreesToRadians(10f);
      model = Matrix4.CreateRotationY(angle);
      // Input updates
    }


  }
}