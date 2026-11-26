namespace Minecraft.Generator
{
    public abstract class TerrainGenerator
    {
        /// <summary>
        /// Creates resources to be used for each chunk
        /// </summary>
        public abstract void CreateResources();

        /// <summary>
        /// Generates a chunk at a given chunk coordinate
        /// </summary>
        /// <param name="chunk">The chunk to be generated</param>
        public abstract void GenerateChunk(Chunk chunk);
    }
}
