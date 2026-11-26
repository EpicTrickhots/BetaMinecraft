using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Minecraft.Graphics;
using Minecraft.Generator.Noise;
using OpenTK.Mathematics;

namespace Minecraft.Generator
{
    public class VoxelTerrainGenerator : IGenerator
    {
        #region Constants (Resolved at compile time)

        public const int WIDTH = 4;

        public const int HEIGHT = 8;

        const int VOXELS_PER_CHUNK = WIDTH * HEIGHT * WIDTH;

        const int BIT_MASK_X = WIDTH - 1;

        const int BIT_MASK_Y = HEIGHT - 1;

        const int Z_SHIFT = WIDTH + HEIGHT;

        const float ISO_LEVEL = 0f;

        #endregion

        private readonly float[] voxels = new float[VOXELS_PER_CHUNK];

        private readonly List<Vertex> vertices = new(1024 * 4);

        private Stopwatch noiseGenTimer = null!;
        private Stopwatch meshGenTimer = null!;

        private FastNoiseLite Noise { get; }

        public VoxelTerrainGenerator(int seed)
        {
            Noise = new FastNoiseLite(seed);

            // Set settings
        }

        public void CreateResources() { }

        public void GenerateChunk(Chunk chunk)
        {
            Vector3i offset = ChunkHelper.ChunkToWorldCoordinate(chunk.coordinate);

            // Construct voxel noise values
            noiseGenTimer = Stopwatch.StartNew();
            GenerateNoiseValues(offset);
            noiseGenTimer.Stop();

            meshGenTimer = Stopwatch.StartNew();
            GenerateChunkMesh(offset);
            meshGenTimer.Stop();
        }

        private void GenerateNoiseValues(Vector3 offset)
        {
            OrderablePartitioner<Tuple<int, int>> paritioner = Partitioner.Create(0, VOXELS_PER_CHUNK);

            Parallel.ForEach(paritioner, range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    // Since our chunk dimensions are powers of 2,
                    // we can use super fast bit-wise operators to un-flatten our index
                    int x = i & BIT_MASK_X;
                    int y = (i >> WIDTH) & BIT_MASK_Y;
                    int z = i >> Z_SHIFT;

                    voxels[i] = Noise.GetNoise(x + offset.X, y, z + offset.Z);
                }
            });
        }

        private void GenerateChunkMesh(Vector3 offset)
        {
            const int STRIDE_Y = 1 << WIDTH;
            const int STRIDE_Z = 1 << Z_SHIFT;

            vertices.Clear();

            var partitioner = Partitioner.Create(0, VOXELS_PER_CHUNK);

            Parallel.ForEach(
                partitioner,
                range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        if (voxels[i] <= ISO_LEVEL)
                            continue;

                        int x = i & BIT_MASK_X;
                        int y = (i >> WIDTH) & BIT_MASK_Y;
                        int z = i >> Z_SHIFT;

                        Vector3 origin = new Vector3(x, y, z) + offset;

                        // Check this voxel's 6 neighboring values to determine which faces to draw
                        if (x == BIT_MASK_X || voxels[i + 1] <= ISO_LEVEL) // +X face
                            AppendFace(origin, 0);

                        if (x == 0 || voxels[i - 1] <= ISO_LEVEL) // -X face
                            AppendFace(origin, 1);

                        if (y == HEIGHT - 1 || voxels[i + STRIDE_Y] <= ISO_LEVEL)
                            AppendFace(origin, 2);

                        if (y == 0 || voxels[i - STRIDE_Y] <= ISO_LEVEL)
                            AppendFace(origin, 3);

                        if (z == WIDTH - 1 || voxels[i + STRIDE_Z] <= ISO_LEVEL)
                            AppendFace(origin, 4);

                        if (z == 0 || voxels[i - STRIDE_Z] <= ISO_LEVEL)
                            AppendFace(origin, 5);
                    }
                }
            );
        }

        private void AppendFace(Vector3 voxel, int faceIndex)
        {
            uint start = (uint)vertices.Count;

            vertices.Add(new Vertex(voxel + faces[faceIndex, 0], new(1f, 0f)));
            vertices.Add(new Vertex(voxel + faces[faceIndex, 1], new(1f, 1f)));
            vertices.Add(new Vertex(voxel + faces[faceIndex, 2], new(0f, 1f)));
            vertices.Add(new Vertex(voxel + faces[faceIndex, 3], new(0f, 0f)));
        }

        private static readonly Vector3[,] faces = new Vector3[6, 4]
        {
            // +X
            { new(0.5f, -0.5f, -0.5f), new(0.5f, 0.5f, -0.5f), new(0.5f, 0.5f, 0.5f), new(0.5f, -0.5f, 0.5f) },

            // -X
            { new(-0.5f, -0.5f, 0.5f), new(-0.5f, 0.5f, 0.5f), new(-0.5f, 0.5f, -0.5f), new(-0.5f, -0.5f, -0.5f) },

            // +Y
            { new(-0.5f, 0.5f, 0.5f), new(0.5f, 0.5f, 0.5f), new(0.5f, 0.5f, -0.5f), new(-0.5f, 0.5f, -0.5f) },

            // -Y
            { new(-0.5f, -0.5f, -0.5f), new(0.5f, -0.5f, -0.5f), new(0.5f, -0.5f, 0.5f), new(-0.5f, -0.5f, 0.5f) },

            // +Z
            { new (0.5f, -0.5f, 0.5f), new (0.5f, 0.5f, 0.5f), new (-0.5f, 0.5f, 0.5f), new (-0.5f, -0.5f, 0.5f) },

            // -Z
            { new(-0.5f, -0.5f, -0.5f), new(-0.5f, 0.5f, -0.5f), new(0.5f, 0.5f, -0.5f), new(0.5f, -0.5f, -0.5f) }
        };
    }
}
