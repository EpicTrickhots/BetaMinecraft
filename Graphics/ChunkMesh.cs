using System;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace Minecraft.Graphics
{
    public class ChunkMesh : IDisposable
    {
        public static int GlobalElementBufferObject { get; private set; }

        public Vertex[] vertices;

        public int VertexBufferObject { get; private set; }
        public int VertexArrayObject { get; private set; }
        public int IndexCount { get; private set; }

        bool disposed = false;

        public static void CreateGlobalIndexBuffer()
        {
            if (GlobalElementBufferObject != 0)
                return;

            // Create global index buffer
        }

        public ChunkMesh(Vertex[] vertices)
        {
            this.vertices = vertices;

            int stride = Unsafe.SizeOf<Vertex>();

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * stride, vertices, BufferUsageHint.StaticDraw);

            // POSITION
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            // TEXCOORD0
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            if (disposed)
                return;

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteBuffer(VertexBufferObject);

            VertexArrayObject = 0;
            VertexBufferObject = 0;
            vertices = null!;

            disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
