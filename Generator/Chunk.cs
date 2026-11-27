using System;
using OpenTK.Mathematics;
using Minecraft.Graphics;

namespace Minecraft.Generator
{
    public class Chunk(Vector3i coordinate) : IDisposable
    {
        /// <summary>
        /// Chunk-space coordinate of this chunk
        /// </summary>
        public Vector3i coordinate = coordinate;

        public Matrix4 worldMatrix = Matrix4.CreateTranslation(ChunkHelper.ChunkToWorldCoordinate(coordinate));

        /// <summary>
        /// Mesh data that will be written to by the mesh generators
        /// </summary>
        public ChunkMesh? mesh;

        private bool disposed = false;

        public void Dispose()
        {
            if (disposed)
                return;

            mesh?.Dispose();
            mesh = null;

            disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
