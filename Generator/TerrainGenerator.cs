using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Minecraft.Graphics;
using Minecraft.Generator.Noise;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;

namespace Minecraft.Generator
{
    public class TerrainGenerator(int seed)
    {
        const int VOXELS_PER_CHUNK = 16 * 256 * 16;

        private readonly float[] voxels = new float[VOXELS_PER_CHUNK];

        private readonly List<Vertex> vertices = new List<Vertex>(32768);

        private Stopwatch noiseGenTimer = null!;
        private Stopwatch meshGenTimer = null!;

        private FastNoiseLite Noise { get; } = new FastNoiseLite(seed);

        public void GenerateChunk(Chunk chunk)
        {
            Vector3i offset = ChunkHelper.ChunkToWorldCoordinate(chunk.coordinate);

            // Construct voxel noise values
            noiseGenTimer = Stopwatch.StartNew();
            GenerateNoiseValues(offset);
            noiseGenTimer.Stop();

            meshGenTimer = Stopwatch.StartNew();
            GenerateChunkMesh();
            meshGenTimer.Stop();
            
            Console.WriteLine($"({chunk.coordinate}) Voxel Gen ({noiseGenTimer.ElapsedMilliseconds}) Mesh Gen ({meshGenTimer.ElapsedMilliseconds})");

            chunk.mesh = new ChunkMesh([.. vertices]);
        }

        private void GenerateNoiseValues(Vector3 offset)
        {
            OrderablePartitioner<Tuple<int, int>> paritioner = Partitioner.Create(0, VOXELS_PER_CHUNK);

            Parallel.ForEach(paritioner, range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Vector3i pos = IndexToVoxelPosition(i);
                    float noise = Noise.GetNoise(pos.X + offset.X, pos.Y, pos.Z + offset.Z);
                    voxels[i] = noise;
                }
            });
        }

        private void GenerateChunkMesh()
        {
            const float ISO = 0f;
            const int X_STRIDE = 1 << 12;
            const int Y_STRIDE = 1 << 4;
            const int Z_STRIDE = 1;

            vertices.Clear();

            for(int i = 0; i < VOXELS_PER_CHUNK; i++)
            {
                if (voxels[i] <= 0f)
                    continue;

                Vector3i pos = IndexToVoxelPosition(i);

                if (pos.X == 15 || voxels[i + X_STRIDE] <= ISO)
                    AppendFace(in pos, 0);

                if (pos.X == 0 || voxels[i - X_STRIDE] <= ISO)
                    AppendFace(in pos, 1);

                if (pos.Y == 255 || voxels[i + Y_STRIDE] <= ISO)
                    AppendFace(in pos, 2);

                if (pos.Y == 0 || voxels[i - Y_STRIDE] <= ISO)
                    AppendFace(in pos, 3);

                if (pos.Z == 15 || voxels[i + Z_STRIDE] <= ISO)
                    AppendFace(in pos, 4);

                if (pos.Z == 0 || voxels[i - Z_STRIDE] <= ISO)
                    AppendFace(in pos, 5);
            }
        }

        /// <summary>
        /// Since we hard-code our chunk dimensions to power of two, we can use bit-wise operators to speed up this calculations :)
        /// </summary>
        /// <param name="index">Index to convert</param>
        /// <returns>A 3D vector representing a voxel in the current chunk</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3i IndexToVoxelPosition(int index)
        {
            const int MASK = 15;

            int x = index >> 12;
            int y = (index >> 4) & 255;
            int z = index & MASK;
            return new Vector3i(x, y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AppendFace(in Vector3i voxel, int faceIndex)
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
