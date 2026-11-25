using Minecraft.Graphics;
using OpenTK.Mathematics;

namespace Minecraft.Generator
{
    public class Chunk(Vector3i coordinate)
    {
        /// <summary>
        /// Chunk-space coordinate of this chunk
        /// </summary>
        public Vector3i coordinate = coordinate;

        /// <summary>
        /// Mesh data that will be written to by the mesh generators
        /// </summary>
        public Mesh? mesh;
    }
}
