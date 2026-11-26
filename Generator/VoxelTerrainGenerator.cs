using Minecraft.Generator.Noise;

using Gen = Minecraft.Generator.IGenerator;

namespace Minecraft.Generator
{
    public class VoxelTerrainGenerator : IGenerator
    {
        const int VOXELS_PER_CHUNK = Gen.CHUNK_WIDTH * Gen.CHUNK_WIDTH * Gen.CHUNK_HEIGHT;

        private readonly float[] voxels = new float[VOXELS_PER_CHUNK];

        private FastNoiseLite Noise { get; }

        public VoxelTerrainGenerator(int seed)
        {
            Noise = new FastNoiseLite(seed);
        }

        public void CreateResources() { }

        public void GenerateChunk(Chunk chunk)
        {
            for(int x = 0; x < Gen.CHUNK_WIDTH; x++)
            {
                for(int y = 0; y < Gen.CHUNK_HEIGHT; y++)
                {
                    for(int z = 0; z < Gen.CHUNK_WIDTH; z++)
                    {
                        int index = (y << 8) | (z << 4) | x;

                        voxels[index] = Noise.GetNoise(x, y, z);
                    }
                }
            }
        }
    }
}
