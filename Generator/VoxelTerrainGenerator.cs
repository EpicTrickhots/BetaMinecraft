using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Minecraft.Graphics;
using OpenTK.Mathematics;

namespace Minecraft.Generator
{
    public class VoxelTerrainGenerator : TerrainGenerator
    {
        private const int NUM_QUADS = CHUNK_WIDTH * CHUNK_WIDTH;

        Vertex[] vertices = new Vertex[NUM_QUADS * 4];

        private uint[] indices = new uint[NUM_QUADS * 6];

        public override void GenerateChunkMesh(Chunk chunk)
        {
            Vector3 offset = ChunkToWorldCoordinate(chunk.coordinate);

            int index = 0;
            for(int x = 0; x < CHUNK_WIDTH - 1; x++)
            {
                for(int z = 0; z < CHUNK_WIDTH - 1; z++)
                {
                    Vector3 v1 = new Vector3(x, 0f, z + 1) + offset;
                    Vector3 v2 = new Vector3(x + 1, 0f, z + 1) + offset;
                    Vector3 v3 = new Vector3(x + 1, 0f, z) + offset;
                    Vector3 v4 = new Vector3(x, 0f, z) + offset;

                    vertices[index] = new Vertex(v1, new(0f, 1f));
                    vertices[index + 1] = new Vertex(v2, new(1f, 1f));
                    vertices[index + 2] = new Vertex(v3, new(1f, 0f));
                    vertices[index + 3] = new Vertex(v4, new(0f, 0f));

                    index += 4;
                }
            }

            chunk.mesh = new Mesh(vertices, indices);
        }

        public override void CreateResources()
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

        public override void ReleaseResources()
        {
            
        }
    }
}
