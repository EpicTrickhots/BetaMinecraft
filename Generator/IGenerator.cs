using Minecraft.Generator.Noise;
using OpenTK.Mathematics;

namespace Minecraft.Generator
{
    public interface IGenerator
    {
        public const int CHUNK_WIDTH = 16;

        public const int CHUNK_HEIGHT = 256;

        /// <summary>
        /// Creates resources to be used for each chunk
        /// </summary>
        public void CreateResources();

        /// <summary>
        /// Generates a chunk at a given chunk coordinate
        /// </summary>
        /// <param name="chunk">The chunk to be generated</param>
        public void GenerateChunk(Chunk chunk);
    }
}
