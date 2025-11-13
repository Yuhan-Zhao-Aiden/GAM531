using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace _2DCollision.Rendering;

internal sealed class ShaderProgram : IDisposable
{
  private readonly int _handle;
  private readonly Dictionary<string, int> _uniformLocations = new();
  private bool _disposed;

  public ShaderProgram(string vertexSource, string fragmentSource)
  {
    var vertex = CompileShader(vertexSource, ShaderType.VertexShader);
    var fragment = CompileShader(fragmentSource, ShaderType.FragmentShader);

    _handle = GL.CreateProgram();
    GL.AttachShader(_handle, vertex);
    GL.AttachShader(_handle, fragment);
    GL.LinkProgram(_handle);

    GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out var status);
    if (status == 0)
    {
      var info = GL.GetProgramInfoLog(_handle);
      throw new InvalidOperationException($"Shader program link failed: {info}");
    }

    GL.DetachShader(_handle, vertex);
    GL.DetachShader(_handle, fragment);
    GL.DeleteShader(vertex);
    GL.DeleteShader(fragment);
  }

  public void Use()
  {
    GL.UseProgram(_handle);
  }

  public void SetMatrix4(string name, Matrix4 value)
  {
    GL.UniformMatrix4(GetLocation(name), false, ref value);
  }

  public void SetVector3(string name, Vector3 value)
  {
    GL.Uniform3(GetLocation(name), value);
  }

  public void Dispose()
  {
    if (_disposed)
    {
      return;
    }

    _disposed = true;
    GL.DeleteProgram(_handle);
  }

  private static int CompileShader(string source, ShaderType type)
  {
    var handle = GL.CreateShader(type);
    GL.ShaderSource(handle, source);
    GL.CompileShader(handle);
    GL.GetShader(handle, ShaderParameter.CompileStatus, out var status);

    if (status == 0)
    {
      var info = GL.GetShaderInfoLog(handle);
      throw new InvalidOperationException($"Failed to compile {type}: {info}");
    }

    return handle;
  }

  private int GetLocation(string name)
  {
    if (_uniformLocations.TryGetValue(name, out var location))
    {
      return location;
    }

    location = GL.GetUniformLocation(_handle, name);
    _uniformLocations[name] = location;
    return location;
  }
}
