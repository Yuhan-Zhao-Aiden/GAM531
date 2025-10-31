using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Monolith.core
{
  public class Shader : IDisposable
  {
    public int handle { get; private set; }

    public Shader(string vPath, string fPath)
    {
      string vsrc = File.ReadAllText(vPath);
      string fsrc = File.ReadAllText(fPath);

      int vs = CompileShader(ShaderType.VertexShader, vsrc);
      int fs = CompileShader(ShaderType.FragmentShader, fsrc);

      int program = GL.CreateProgram();
      GL.AttachShader(program, vs);
      GL.AttachShader(program, fs);
      GL.LinkProgram(program);

      GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int linked);
      if (linked == 0)
      {
        GL.DeleteProgram(program);
        GL.DeleteShader(vs);
        GL.DeleteShader(fs);
        throw new Exception("Shader program failed to link");
      }

      GL.DetachShader(program, vs);
      GL.DetachShader(program, fs);
      GL.DeleteShader(vs);
      GL.DeleteShader(fs);

      this.handle = program;
    }


    private static int CompileShader(ShaderType type, string source)
    {
      int shader = GL.CreateShader(type);
      GL.ShaderSource(shader, source);
      GL.CompileShader(shader);
      GL.GetShader(shader, ShaderParameter.CompileStatus, out int ok);
      if (ok == 0)
      {
        GL.DeleteShader(shader);
        throw new Exception($"{type} compile failed");
      }
      return shader;
    }

    public void Use() => GL.UseProgram(handle);

    public void SetInt(string name, int v) =>
        GL.Uniform1(GL.GetUniformLocation(handle, name), v);
    public void SetFloat(string name, float v) =>
        GL.Uniform1(GL.GetUniformLocation(handle, name), v);
    public void SetBool(string name, bool v) =>
        GL.Uniform1(GL.GetUniformLocation(handle, name), v ? 1 : 0);
    public void SetVector3(string name, in Vector3 v) =>
        GL.Uniform3(GL.GetUniformLocation(handle, name), v);
    public void SetMatrix4(string name, Matrix4 m, bool transpose = false) =>
        GL.UniformMatrix4(GL.GetUniformLocation(handle, name), transpose, ref m);

    public void Dispose()
    {
      if (handle != 0)
      {
        GL.DeleteProgram(handle);
        handle = 0;
      }
    }
  }
}