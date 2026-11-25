using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace Minecraft.Graphics
{
    public class Shader : IDisposable
    {
        public int vertex { get; }

        public int fragment { get; }

        public int handle { get; }

        public Dictionary<string, int> uniformLocations;

        public int this[string name] => uniformLocations.TryGetValue(name, out int v) ? v : -1;

        private bool disposedValue = false;

        public Shader(string vsource, string fsource)
        {
            vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vsource);

            fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fsource);

            GL.CompileShader(vertex);

            GL.GetShader(vertex, ShaderParameter.CompileStatus, out int vsuccess);
            if (vsuccess == 0)
            {
                string infoLog = GL.GetShaderInfoLog(vertex);
                Console.WriteLine(infoLog);
            }

            GL.CompileShader(fragment);

            GL.GetShader(fragment, ShaderParameter.CompileStatus, out int fsuccess);
            if (fsuccess == 0)
            {
                string infoLog = GL.GetShaderInfoLog(fragment);
                Console.WriteLine(infoLog);
            }

            handle = GL.CreateProgram();

            GL.AttachShader(handle, vertex);
            GL.AttachShader(handle, fragment);
            GL.LinkProgram(handle);

            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int hsuccess);
            if (hsuccess == 0)
            {
                string infoLog = GL.GetProgramInfoLog(handle);
                Console.WriteLine(infoLog);
            }

            GL.DetachShader(handle, vertex);
            GL.DetachShader(handle, fragment);
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);

            GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms, out int numUniforms);

            uniformLocations = [];

            for (int i = 0; i < numUniforms; i++)
            {
                string name = GL.GetActiveUniform(handle, i, out _, out _);
                int loc = GL.GetUniformLocation(handle, name);
                uniformLocations.Add(name, loc);
            }
        }

        public virtual void Use()
        {
            GL.UseProgram(handle);
        }

        public static Shader Load(string dir, string name)
        {
            string vertexPath = Path.Combine(dir, $"{name}.vert");
            string fragmentPath = Path.Combine(dir, $"{name}.frag");

            string vertexCode = File.ReadAllText(vertexPath);
            string fragmentCode = File.ReadAllText(fragmentPath);

            return new Shader(vertexCode, fragmentCode);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(handle);
                disposedValue = true;
            }
        }

        ~Shader()
        {
            if (!disposedValue)
            {
                Console.WriteLine("GPU resource leak! Did you forget to call Dispose()?");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
