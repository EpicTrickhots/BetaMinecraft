namespace Minecraft.Generator
{
    public interface IGenerator
    {
        

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
