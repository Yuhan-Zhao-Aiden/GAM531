using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace A9.Graphics;

public class Texture : IDisposable
{
  public int Handle { get; private set; }
  private bool _disposed = false;

  public Texture(string path)
  {
    Handle = GL.GenTexture();
    Use();

    StbImage.stbi_set_flip_vertically_on_load(1);

    using (Stream stream = File.OpenRead(path))
    {
      ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
          image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
    }

    // Set texture parameters
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
  }

  public void Use(TextureUnit unit = TextureUnit.Texture0)
  {
    GL.ActiveTexture(unit);
    GL.BindTexture(TextureTarget.Texture2D, Handle);
  }

  public void Bind(TextureUnit unit = TextureUnit.Texture0)
  {
    Use(unit);
  }

  public void Dispose()
  {
    if (!_disposed)
    {
      GL.DeleteTexture(Handle);
      _disposed = true;
    }
    GC.SuppressFinalize(this);
  }
}
