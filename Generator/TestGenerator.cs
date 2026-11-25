using System.Collections.Generic;
using Minecraft.Graphics;
using OpenTK.Mathematics;

namespace Minecraft.Generator
{
    public class TestGenerator : TerrainGenerator
    {
        private const int NUM_QUADS = CHUNK_SIZE * CHUNK_SIZE;

        Vertex[] vertices = new Vertex[NUM_QUADS * 4];

        private uint[] indices = new uint[NUM_QUADS * 6];

        public override void GenerateChunkMesh(Chunk chunk)
        {
            Vector3 offset = ChunkToWorldCoordinate(chunk.coordinate);

            int index = 0;
            for(int x = 0; x < CHUNK_SIZE - 1; x++)
            {
                for(int z = 0; z < CHUNK_SIZE - 1; z++)
                {
                    Vertex v1 = new Vertex(x, 0f, z + 1, 0f, 1f);
                    Vertex v2 = new Vertex(x + 1, 0f, z + 1, 1f, 1f);
                    Vertex v3 = new Vertex(x + 1, 0f, z, 1f, 0f);
                    Vertex v4 = new Vertex(x, 0f, z, 0f, 0f);

                    vertices[index] = v1;
                    vertices[index + 1] = v2;
                    vertices[index + 2] = v3;
                    vertices[index + 3] = v4;

                    index += 4;
                }

                index += 4;
            }

            chunk.mesh = new Mesh(vertices, indices);
        }

        public override void CreateGpuResources()
        {
            for (uint i = 0; i < NUM_QUADS; i++)
            {
                uint qv = i * 4;
                uint qi = i * 6;

                indices[qi + 0] = qv + 0;
                indices[qi + 1] = qv + 1;
                indices[qi + 2] = qv + 2;
                indices[qi + 3] = qv + 2;
                indices[qi + 4] = qv + 3;
                indices[qi + 5] = qv + 0;
            }
        }

        public override void ReleaseGpuResources()
        {
            
        }
    }
}
