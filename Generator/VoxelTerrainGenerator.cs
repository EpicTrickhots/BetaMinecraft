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
    public class VoxelTerrainGenerator : TerrainGenerator
    {
        const int VOXELS_PER_CHUNK = 16 * 256 * 16;

        const int Z_SHIFT = 12;

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

        public override void CreateResources()
        {
            // Create largest possible index array for a chunk


        }

        public override void GenerateChunk(Chunk chunk)
        {
            Vector3i offset = ChunkHelper.ChunkToWorldCoordinate(chunk.coordinate);

            // Construct voxel noise values
            noiseGenTimer = Stopwatch.StartNew();
            GenerateNoiseValues(offset);
            noiseGenTimer.Stop();

            meshGenTimer = Stopwatch.StartNew();
            GenerateChunkMesh(offset);
            meshGenTimer.Stop();

            Console.WriteLine();
        }

        private void GenerateNoiseValues(Vector3 offset)
        {
            OrderablePartitioner<Tuple<int, int>> paritioner = Partitioner.Create(0, VOXELS_PER_CHUNK);

            Parallel.ForEach(paritioner, range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Vector3i pos = IndexToVoxelPosition(i);
                    voxels[i] = Noise.GetNoise(pos.X + offset.X, pos.Y, pos.Z + offset.Z);
                }
            });
        }

        private void GenerateChunkMesh(Vector3 offset)
        {
            const int Z_STRIDE = 1 << Z_SHIFT;
            const int Y_STRIDE = 1 << 4;

            vertices.Clear();

            var partitioner = Partitioner.Create(0, VOXELS_PER_CHUNK);

            Parallel.ForEach(
                partitioner,
                range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        // Voxel value is negative -> air block
                        if (voxels[i] <= 0f)
                            continue;

                        Vector3i pos = IndexToVoxelPosition(i);
                        Vector3 origin = pos + offset;

                        // Check this voxel's 6 neighboring values to determine which faces to draw
                        if (pos.X == 15 || voxels[i + 1] <= 0f) // +X face
                            AppendFace(origin, 0);

                        if (pos.X == 0 || voxels[i - 1] <= 0f) // -X face
                            AppendFace(origin, 1);

                        if (pos.Y == 255 || voxels[i + Y_STRIDE] <= 0f)
                            AppendFace(origin, 2);

                        if (pos.Y == 0 || voxels[i - Y_STRIDE] <= 0f)
                            AppendFace(origin, 3);

                        if (pos.Z == 15 || voxels[i + Z_STRIDE] <= 0f)
                            AppendFace(origin, 4);

                        if (pos.Z == 0 || voxels[i - Z_STRIDE] <= 0f)
                            AppendFace(origin, 5);
                    }
                }
            );
        }

        /// <summary>
        /// Since we hard-code our chunk dimensions to power of two, we can use bit-wise operators to speed up this calculations :)
        /// </summary>
        /// <param name="index">Index to convert</param>
        /// <returns>A 3D vector representing a voxel in the current chunk</returns>
        private static Vector3i IndexToVoxelPosition(int index)
        {
            const int MASK_X = 15;
            const int MASK_Y = 255;

            int x = index & MASK_X;
            int y = (index >> 4) & MASK_Y;
            int z = index >> Z_SHIFT;
            return new Vector3i(x, y, z);
        }

        private void AppendFace(Vector3 voxel, int faceIndex)
        {
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
