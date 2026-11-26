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
            Vector3i offset = ChunkHelper.ChunkToWorldCoordinate(chunk.coordinate);

            // Construct voxel noise values
            noiseGenTimer = Stopwatch.StartNew();
            GenerateNoiseValues(offset);
            noiseGenTimer.Stop();

            meshGenTimer = Stopwatch.StartNew();
            GenerateChunkMesh(offset);
            meshGenTimer.Stop();

            // Console.WriteLine($"({chunk.coordinate}) Voxel Gen ({noiseGenTimer.ElapsedMilliseconds}) Mesh Gen ({meshGenTimer.ElapsedMilliseconds})");

            chunk.mesh = new ChunkMesh([.. vertices]);
        }

        private void GenerateNoiseValues(Vector3 offset)
        {
            for(int i = 0; i < VOXELS_PER_CHUNK; i++)
            {
                Vector3 samplePosition = TerrainGenerator.IndexToVoxelPosition(i);
            }
        }

        private void GenerateChunkMesh(Vector3 offset)
        {

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
}
