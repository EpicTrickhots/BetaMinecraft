using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Minecraft.Generator.Noise;
using OpenTK.Mathematics;

using Gen = Minecraft.Generator.IGenerator;

namespace Minecraft.Generator
{
    public class VoxelTerrainGenerator : IGenerator
    {
        const int VOXELS_PER_CHUNK = Gen.CHUNK_WIDTH * Gen.CHUNK_WIDTH * Gen.CHUNK_HEIGHT;

        const int WIDTH_M = Gen.CHUNK_WIDTH - 1;

        const int HEIGHT_M = Gen.CHUNK_HEIGHT - 1;

        const int Z_SHIFT = Gen.WIDTH_SHIFT + Gen.HEIGHT_SHIFT;

        private readonly float[] voxels = new float[VOXELS_PER_CHUNK];

        private FastNoiseLite Noise { get; }

        public VoxelTerrainGenerator(int seed)
        {
            Noise = new FastNoiseLite(seed);

            // Set settings
        }

        public void CreateResources() { }

        public void GenerateChunk(Chunk chunk)
        {
            Vector3 offset = ChunkHelper.ChunkToWorldCoordinate(chunk.coordinate);

            OrderablePartitioner<Tuple<int, int>> rangePartitioner = Partitioner.Create(0, VOXELS_PER_CHUNK);

            Parallel.ForEach(rangePartitioner, range =>
            {
                for(int i = range.Item1; i < range.Item2; i++)
                {
                    // Since our chunk dimensions are powers of 2,
                    // we can use super fast bit-wise operators to un-flatten our index
                    int x = (i & WIDTH_M);
                    int y = (i >> Gen.WIDTH_SHIFT) & HEIGHT_M;
                    int z = (i >> Z_SHIFT);

                    voxels[i] = Noise.GetNoise(x + offset.X, y, z + offset.Z);
                }
            });
        }
    }
}
