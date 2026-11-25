using Minecraft.Graphics;
using OpenTK.Mathematics;

namespace Minecraft.Generator
{
    public class TestGenerator : TerrainGenerator
    {
        Vertex[] vertices;
        uint[] indices;

        public override void GenerateChunkMesh(Chunk chunk)
        {
            
        }

        public override void CreateGpuResources()
        {
            int numQuads = (CHUNK_SIZE - 1) * (CHUNK_SIZE - 1);

            vertices = new Vertex[numQuads * 4];
            indices = new uint[numQuads * 6];

            for (uint i = 0; i < numQuads; i++)
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
