using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace A9.Graphics;

public class Shader : IDisposable
{
  public int Handle { get; private set; }
  private bool _disposed = false;
  private Dictionary<string, int> _uniformLocations;

  public Shader(string vertexPath, string fragmentPath)
  {
    string vertexShaderSource = File.ReadAllText(vertexPath);
    string fragmentShaderSource = File.ReadAllText(fragmentPath);

    int vertexShader = GL.CreateShader(ShaderType.VertexShader);
    GL.ShaderSource(vertexShader, vertexShaderSource);
    GL.CompileShader(vertexShader);
    CheckCompileErrors(vertexShader, "VERTEX");

    int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
    GL.ShaderSource(fragmentShader, fragmentShaderSource);
    GL.CompileShader(fragmentShader);
    CheckCompileErrors(fragmentShader, "FRAGMENT");

    Handle = GL.CreateProgram();
    GL.AttachShader(Handle, vertexShader);
    GL.AttachShader(Handle, fragmentShader);
    GL.LinkProgram(Handle);
    CheckCompileErrors(Handle, "PROGRAM");

    GL.DeleteShader(vertexShader);
    GL.DeleteShader(fragmentShader);

    _uniformLocations = new Dictionary<string, int>();
    GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int uniformCount);
    for (int i = 0; i < uniformCount; i++)
    {
      string name = GL.GetActiveUniform(Handle, i, out _, out _);
      int location = GL.GetUniformLocation(Handle, name);
      _uniformLocations.Add(name, location);
    }
  }

  public void Use()
  {
    GL.UseProgram(Handle);
  }

  public int GetUniformLocation(string name)
  {
    if (_uniformLocations.TryGetValue(name, out int location))
      return location;

    location = GL.GetUniformLocation(Handle, name);
    _uniformLocations[name] = location;
    return location;
  }

  // Uniform setters
  public void SetMatrix4(string name, Matrix4 matrix)
  {
    int location = GetUniformLocation(name);
    GL.UniformMatrix4(location, false, ref matrix);
  }

  public void SetVector3(string name, Vector3 vector)
  {
    int location = GetUniformLocation(name);
    GL.Uniform3(location, vector);
  }

  public void SetFloat(string name, float value)
  {
    int location = GetUniformLocation(name);
    GL.Uniform1(location, value);
  }

  public void SetInt(string name, int value)
  {
    int location = GetUniformLocation(name);
    GL.Uniform1(location, value);
  }

  private void CheckCompileErrors(int shader, string type)
  {
    if (type != "PROGRAM")
    {
      GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
      if (success == 0)
      {
        string infoLog = GL.GetShaderInfoLog(shader);
        throw new Exception($"Shader compilation error ({type}):\n{infoLog}");
      }
    }
    else
    {
      GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out int success);
      if (success == 0)
      {
        string infoLog = GL.GetProgramInfoLog(shader);
        throw new Exception($"Shader program linking error:\n{infoLog}");
      }
    }
  }

  public void Dispose()
  {
    if (!_disposed)
    {
      GL.DeleteProgram(Handle);
      _disposed = true;
    }
    GC.SuppressFinalize(this);
  }
}
