using OpenTK.Mathematics;

namespace Minecraft.Generator
{
    public static class ChunkHelper
    {
        /// <summary>
        /// Converts a chunk coordinate into its world-space coordinate
        /// </summary>
        /// <param name="a">Input chunk coordinate</param>
        /// <returns>World-space position</returns>
        public static Vector3i ChunkToWorldCoordinate(Vector3i a)
        {
            int x = a.X << IGenerator.WIDTH_SHIFT;
            int z = a.Z << IGenerator.WIDTH_SHIFT;
            return new Vector3i(x, 0f, z);
        }

        /// <summary>
        /// Converts a world-space position into the corresponding chunk coordinate
        /// </summary>
        /// <param name="a">Input world-space coordinate</param>
        /// <returns>Chunk coordinate</returns>
        public static Vector3i WorldToChunkCoordinate(Vector3 a)
        {
            int x = (int)MathHelper.Floor(a.X / IGenerator.CHUNK_WIDTH);
            int z = (int)MathHelper.Floor(a.Z / IGenerator.CHUNK_WIDTH);
            return new Vector3i(x, 0, z);
        }
    }
}
