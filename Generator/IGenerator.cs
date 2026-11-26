namespace Minecraft.Generator
{
    public interface IGenerator
    {
        public const int CHUNK_WIDTH = 1 << 4;

        public const int CHUNK_HEIGHT = 1 << 8;

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
