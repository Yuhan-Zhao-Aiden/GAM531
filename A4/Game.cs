using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ApplyTexture
{
  public class Game : GameWindow
  {

    private int vao, vbo, ebo;
    private float length = 2.0f, side = 1.0f;

    public Game(
      GameWindowSettings gs,
      NativeWindowSettings ns
    ) : base(gs, ns) { }

    protected override void OnLoad()
    {
      base.OnLoad();
      GL.ClearColor(0.12f, 0.12f, 0.12f, 1.0f);
      GL.Enable(EnableCap.DepthTest);

      var (vertices, indices) = Utility.BuildCuboid(length, side);

      vao = GL.GenVertexArray();
      GL.BindBuffer(BufferTarget.ArrayBuffer, vao);

      // set up vbo
      vbo = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
      GL.BufferData(BufferTarget.ArrayBuffer,
        vertices.Length * sizeof(float),
        vertices, BufferUsageHint.StaticDraw);

      // set up ebo (tell gpu which vertex for each triangle)
      ebo = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
      GL.BufferData(BufferTarget.ElementArrayBuffer,
        indices.Length * sizeof(uint),
        indices, BufferUsageHint.StaticDraw);

      GL.EnableVertexAttribArray(0); // first attrib (position)
      GL.VertexAttribPointer(
        index: 0,
        size: 3,
        type: VertexAttribPointerType.Float,
        normalized: false,
        stride: 5 * sizeof(float),
        offset: 0
      );

      GL.EnableVertexAttribArray(1); // second attrib (uv coordinates)
      GL.VertexAttribPointer(
        index: 1,
        size: 2,
        type: VertexAttribPointerType.Float,
        normalized: false,
        stride: 5 * sizeof(float),
        offset: 3 * sizeof(float)
      );

      GL.BindVertexArray(0);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      SwapBuffers();
    }

    protected override void OnUnload()
    {
      base.OnUnload();
      GL.BindVertexArray(0);
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

      GL.DeleteBuffer(ebo);
      GL.DeleteBuffer(vbo);
      GL.DeleteVertexArray(vao);
    }
  }
}