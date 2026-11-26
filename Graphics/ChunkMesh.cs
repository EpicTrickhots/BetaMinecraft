using System;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace Minecraft.Graphics
{
    public class ChunkMesh : IDisposable
    {
        // This number represents the absurd but possible case where every block in a chunk is in a 3D checkerboard pattern.
        // This case is unique in that it creates the highest possible number of rendered faces in a single chunk.
        // Since all quads are drawn with the same indices, chunk meshes don't actually need to store their own indices,
        // and we can re-use the same global buffer for all chunk meshes.

        // This does result in an array that contains a stupid 1,179,648 integers which uses about ~4.5 MB of memory.
        // Still better than storing index arrays for every chunk
        const int MAX_QUADS_PER_CHUNK = 16 * 256 * 16 / 2 * 6;
        const int MAX_INDICES_PER_CHUNK = MAX_QUADS_PER_CHUNK * 6;

        public static int GlobalElementBufferObject { get; private set; } = 0;

        public Vertex[] vertices;

        public int VertexBufferObject { get; private set; }
        public int VertexArrayObject { get; private set; }
        public int IndexCount { get; private set; }

        bool disposed = false;

        public static void CreateGlobalIndexBuffer()
        {
            if (GlobalElementBufferObject != 0)
                return;

            uint[] indices = new uint[MAX_INDICES_PER_CHUNK];

            for (uint q = 0, v = 0, i = 0; q < MAX_QUADS_PER_CHUNK; q++)
            {
                indices[i + 0] = v + 0;
                indices[i + 1] = v + 1;
                indices[i + 2] = v + 2;
                indices[i + 3] = v + 2;
                indices[i + 4] = v + 3;
                indices[i + 5] = v + 0;

                v += 4;
                i += 6;
            }

            GlobalElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, GlobalElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public ChunkMesh(Vertex[] vertices)
        {
            if(GlobalElementBufferObject == 0)
                throw new InvalidOperationException("Cannot create chunk mesh before global index buffer is created!");

            this.vertices = vertices;
            IndexCount = vertices.Length / 4 * 4;

            if (IndexCount > MAX_INDICES_PER_CHUNK)
                throw new ArgumentException($"Chunk indices exceeds global capacity ({IndexCount} indices)");

            int stride = Unsafe.SizeOf<Vertex>();

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * stride, vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, GlobalElementBufferObject);

            // POSITION
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            // TEXCOORD0
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Dispose()
        {
            if (disposed)
                return;

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteBuffer(VertexBufferObject);

            // Idk if this is necessary but whatever
            VertexArrayObject = 0;
            VertexBufferObject = 0;
            vertices = null!;

            disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
