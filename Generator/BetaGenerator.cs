using Minecraft.Generator.Noise;
using Minecraft.Graphics;
using OpenTK.Mathematics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.Generator
{
    public class BetaGenerator(int seed)
    {
        const int VOXELS_PER_CHUNK = 16 * 256 * 16;

        private readonly float[] voxels = new float[VOXELS_PER_CHUNK];

        private readonly List<Vertex> vertices = new List<Vertex>(4096);

        private Stopwatch noiseGenTimer = null!;
        private Stopwatch meshGenTimer = null!;

        public int Seed { get; } = seed;
        private FastNoiseLite Noise { get; } = new FastNoiseLite(seed);

        public void GenerateChunk(Chunk chunk)
        {
            vertices.Clear();
            Vector3i offset = ChunkHelper.ChunkToWorldCoordinate(chunk.coordinate);

            // Construct voxel noise values
            noiseGenTimer = Stopwatch.StartNew();
            GenerateNoiseValues(offset);
            noiseGenTimer.Stop();

            meshGenTimer = Stopwatch.StartNew();
            GenerateChunkMesh(offset);
            meshGenTimer.Stop();

            Console.WriteLine($"({chunk.coordinate}) Voxel Gen ({noiseGenTimer.ElapsedMilliseconds}) Mesh Gen ({meshGenTimer.ElapsedMilliseconds})");

            chunk.mesh = new ChunkMesh([.. vertices]);
        }

        private void GenerateNoiseValues(Vector3 offset)
        {
            for(int i = 0; i < VOXELS_PER_CHUNK; i++)
            {
                Vector3i pos = TerrainGenerator.IndexToVoxelPosition(i);
                float noise = Noise.GetNoise(pos.X + offset.X, pos.Y, pos.Z + offset.Z);
                voxels[i] = noise;
            }
        }

        private void GenerateChunkMesh(Vector3 offset)
        {
            const int Z_STRIDE = 1 << 12;
            const int Y_STRIDE = 1 << 4;
            const float ISO = 0f;

            for(int x = 0; x < 16; x++)
            {
                for (int y = 0; x < 256; x++)
                {
                    for (int z = 0; x < 16; x++)
                    {
                        int i = (y << 8) | (z << 4) | x;
                        if (voxels[i] <= ISO)
                            return;

                        // It is probably faster to use a 1D for() loop and then do the bit shifting from earlier to get x, y, and z.
                        Vector3 pos = new Vector3i(x, y, z);

                        // Check this voxel's 6 neighboring values to determine which faces to draw
                        if (x == 15 || voxels[i + 1] <= 0f) // +X face
                            AppendFace(pos, 0);

                        if (x == 0 || voxels[i - 1] <= 0f) // -X face
                            AppendFace(pos, 1);

                        if (y == 255 || voxels[i + Y_STRIDE] <= 0f)
                            AppendFace(pos, 2);

                        if (y == 0 || voxels[i - Y_STRIDE] <= 0f)
                            AppendFace(pos, 3);

                        if (z == 15 || voxels[i + Z_STRIDE] <= 0f)
                            AppendFace(pos, 4);

                        if (z == 0 || voxels[i - Z_STRIDE] <= 0f)
                            AppendFace(pos, 5);
                    }
                }
            }
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
