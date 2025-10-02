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
    private float length = 1.0f, side = 0.5f;
    private int shader, indexLength;
    private int texture;

    // For MVP
    private int uMvp;
    private Matrix4 model, view, proj;
    private float angle = 0f;

    // shader src code
    private const string vertexSrc = @"
    #version 330 core
    layout (location = 0) in vec3 aPos;
    layout (location = 1) in vec2 aUV;

    out vec2 vUV;

    uniform mat4 uMVP;

    void main()
    {
        vUV = aUV;
        gl_Position = uMVP * vec4(aPos, 1.0);
    }";

    // Apply texture using uv
    private const string fragmentSrc = @"
    #version 330 core
    in vec2 vUV;
    out vec4 FragColor;

    uniform sampler2D uTex;

    void main()
    {
        FragColor = texture(uTex, vUV);
    }";


    // FragColor = texture(uTex, vUV);


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
      indexLength = indices.Length;

      vao = GL.GenVertexArray();
      // GL.BindBuffer(BufferTarget.ArrayBuffer, vao);
      GL.BindVertexArray(vao);

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


      // Load crate texture
      texture = GL.GenTexture();
      GL.ActiveTexture(TextureUnit.Texture0);
      GL.BindTexture(TextureTarget.Texture2D, texture);

      // Setting up sampling (What to do when texture smaller than the actual size...)
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

      StbImageSharp.ImageResult image;
      using (var stream = File.OpenRead("Assets/crate.png")) // Get the texture from the image
      {
        StbImageSharp.StbImage.stbi_set_flip_vertically_on_load(1);
        image = StbImageSharp.ImageResult.FromStream(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);
      }

      // Image is uploaded to gpu
      GL.TexImage2D(TextureTarget.Texture2D, level: 0, internalformat: PixelInternalFormat.Rgba,
              width: image.Width, height: image.Height, border: 0,
              format: PixelFormat.Rgba, type: PixelType.UnsignedByte, pixels: image.Data);

      GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

      // shaders
      shader = CreateShaderProgram(vertexSrc, fragmentSrc);

      GL.UseProgram(shader);
      int texLoc = GL.GetUniformLocation(shader, "uTex");
      GL.Uniform1(texLoc, 0);
      GL.UseProgram(0);

      // setup mvp
      GL.UseProgram(shader);
      uMvp = GL.GetUniformLocation(shader, "uMVP");
      GL.UseProgram(0);

      view = Matrix4.LookAt(
          new Vector3(0f, 1f, 3f),
          Vector3.Zero,
          Vector3.UnitY);

      float aspect = Size.X / (float)Size.Y;
      proj = Matrix4.CreatePerspectiveFieldOfView(
          MathHelper.DegreesToRadians(60f),
          aspect, 0.1f, 100f);

      model = Matrix4.Identity;

    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);
      GL.Viewport(0, 0, Size.X, Size.Y);

      float aspect = Size.X / (float)Size.Y;
      proj = Matrix4.CreatePerspectiveFieldOfView(
          MathHelper.DegreesToRadians(60f),
          aspect, 0.1f, 100f);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      var mvp = model * view * proj; // calculate mvp matrix

      GL.UseProgram(shader);
      GL.UniformMatrix4(uMvp, false, ref mvp);

      GL.ActiveTexture(TextureUnit.Texture0);
      GL.BindTexture(TextureTarget.Texture2D, texture);

      GL.BindVertexArray(vao);
      GL.DrawElements(PrimitiveType.Triangles, indexLength, DrawElementsType.UnsignedInt, 0);

      SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);
      angle += (float)args.Time * MathHelper.DegreesToRadians(45f);
      model = Matrix4.CreateRotationY(angle);
    }

    // This function to create the program using vertex and frag src code
    private static int CreateShaderProgram(string vertexSrc, string fragmentSrc)
    {
      int vs = GL.CreateShader(ShaderType.VertexShader);
      GL.ShaderSource(vs, vertexSrc);
      GL.CompileShader(vs);
      GL.GetShader(vs, ShaderParameter.CompileStatus, out int vStatus);
      if (vStatus == 0) throw new Exception("Something wrong when compiling vertex shader!");

      int fs = GL.CreateShader(ShaderType.FragmentShader);
      GL.ShaderSource(fs, fragmentSrc);
      GL.CompileShader(fs);
      GL.GetShader(fs, ShaderParameter.CompileStatus, out int fStatus);
      if (fStatus == 0) throw new Exception("Something wrong when compiling fragment shader!");

      int program = GL.CreateProgram();
      GL.AttachShader(program, vs);
      GL.AttachShader(program, fs);
      GL.LinkProgram(program);
      GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int linkStatus);
      if (linkStatus == 0) throw new Exception("Something wrong when linking");

      GL.DetachShader(program, vs);
      GL.DetachShader(program, fs);
      GL.DeleteShader(vs);
      GL.DeleteShader(fs);
      return program;
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

      if (shader != 0) GL.DeleteProgram(shader);
      if (texture != 0) GL.DeleteTexture(texture);
    }
  }
}