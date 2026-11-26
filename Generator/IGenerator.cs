namespace Minecraft.Generator
{
    public interface IGenerator
    {
        public const int WIDTH_SHIFT = 4;

        public const int HEIGHT_SHIFT = 8;

        public const int CHUNK_WIDTH = 1 << WIDTH_SHIFT;

        public const int CHUNK_HEIGHT = 1 << HEIGHT_SHIFT;

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
