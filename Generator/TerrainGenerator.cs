using OpenTK.Mathematics;

namespace Minecraft.Generator
{
    public abstract class TerrainGenerator
    {
        protected const int THREAD_GROUP_SIZE = 8;

        public const int CHUNK_SIZE = 16;

        /// <summary>
        /// Creates GPU buffers to be used for each chunk
        /// </summary>
        public abstract void CreateResources();

        /// <summary>
        /// Releases our GPU buffers to avoid memory leaks
        /// </summary>
        public abstract void ReleaseResources();

        /// <summary>
        /// Generates a chunk at a given chunk coordinate
        /// </summary>
        /// <param name="chunk">The chunk to be generated</param>
        public abstract void GenerateChunkMesh(Chunk chunk);

        /// <summary>
        /// Converts a chunk coordinate into its world-space coordinate
        /// </summary>
        /// <param name="a">Input chunk coordinate</param>
        /// <returns>World-space position</returns>
        public static Vector3 ChunkToWorldCoordinate(Vector3i a)
        {
            float x = a.X * (CHUNK_SIZE - 1);
            float z = a.Z * (CHUNK_SIZE - 1);
            return new Vector3(x, 0f, z);
        }

        /// <summary>
        /// Converts a world-space position into the corresponding chunk coordinate
        /// </summary>
        /// <param name="a">Input world-space coordinate</param>
        /// <returns>Chunk coordinate</returns>
        public static Vector3i WorldToChunkCoordinate(Vector3 a)
        {
            int x = (int)MathHelper.Floor(a.X / CHUNK_SIZE);
            int z = (int)MathHelper.Floor(a.Z / CHUNK_SIZE);
            return new Vector3i(x, 0, z);
        }
    }
}
