using OpenTK.Graphics.OpenGL4;
using System.Runtime.CompilerServices;

namespace Minecraft.Graphics
{
    public class ChunkMesh
    {
        public Vertex[] vertices;

        public uint[] indices;

        public int VertexBufferObject { get; private set; }
        public int VertexArrayObject { get; private set; }
        public int ElementBufferObject { get; private set; }

        public ChunkMesh(Vertex[] vertices, uint[] indices)
        {
            this.vertices = vertices;
            this.indices = indices;

            int stride = Unsafe.SizeOf<Vertex>();

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * stride, vertices, BufferUsageHint.StaticDraw);

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // POSITION
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            // TEXCOORD0
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }
    }
}
