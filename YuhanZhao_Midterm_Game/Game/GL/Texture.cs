using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using System;
using System.IO;

namespace Monolith.core
{
  public class Texture : IDisposable
  {
    public int Handle { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public Texture(string path)
    {
      Handle = GL.GenTexture();
      GL.BindTexture(TextureTarget.Texture2D, Handle);

      // Load image using StbImageSharp
      StbImage.stbi_set_flip_vertically_on_load(1); // Flip texture coordinates

      using (var stream = File.OpenRead(path))
      {
        ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        
        Width = image.Width;
        Height = image.Height;

        // Upload texture data to GPU
        GL.TexImage2D(
          TextureTarget.Texture2D,
          0,
          PixelInternalFormat.Rgba,
          image.Width,
          image.Height,
          0,
          PixelFormat.Rgba,
          PixelType.UnsignedByte,
          image.Data
        );
      }

      // Set texture parameters
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

      // Generate mipmaps
      GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

      // Unbind texture
      GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
      GL.ActiveTexture(unit);
      GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Dispose()
    {
      if (Handle != 0)
      {
        GL.DeleteTexture(Handle);
        Handle = 0;
      }
    }
  }
}