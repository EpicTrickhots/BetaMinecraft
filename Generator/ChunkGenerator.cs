using System;
using System.Diagnostics;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Minecraft.Generator
{
    internal class ChunkGenerator(TerrainGenerator generator)
    {
        /// <summary>
        /// Radius of the rendered chunk circle around the viewer
        /// </summary>
        public const int RENDER_DISTANCE = 8;

        /// <summary>
        /// Maximum number of chunks that can be generated in a single chunk update
        /// </summary>
        public const int MAX_CHUNKS_PER_UPDATE = 2;

        /// <summary>
        /// Position to base render distance off of
        /// </summary>
        public Vector3 viewPosition;

        /// <summary>
        /// Terrain generator that will handle mesh generation
        /// </summary>
        public TerrainGenerator Generator { get; } = generator;

        /// <summary>
        /// Chunk coordinate that the viewer is currently standing in
        /// </summary>
        private Vector3i viewerChunk = new Vector3i();

        /// <summary>
        /// All chunks that have been generated, active or not
        /// </summary>
        public readonly Dictionary<Vector3i, Chunk> cachedChunks = [];

        /// <summary>
        /// Chunks that have been generated, and are currently rendered
        /// </summary>
        public readonly List<Vector3i> renderedChunks = [];

        /// <summary>
        /// Chunks that are queued for rendering/generation
        /// </summary>
        public readonly Queue<Vector3i> queuedChunks = [];

        //private Stopwatch debugTimer;

        public void Initialize()
        {
            Generator.CreateGpuResources();
            CalculateNeededChunks();
        }

        public void Update()
        {
            Vector3i cc = TerrainGenerator.WorldToChunkCoordinate(viewPosition);
            if(cc != viewerChunk)
            {
                viewerChunk = cc;
                CalculateNeededChunks();
            }

            RequestChunkUpdate();
        }

        public void CleanUp()
        {
            Generator.ReleaseGpuResources();
        }

        private void CreateChunk(Vector3i coordinate)
        {
            Chunk chunk = new(coordinate);
            Generator.GenerateChunkMesh(chunk);

            cachedChunks.Add(coordinate, chunk);
            renderedChunks.Add(coordinate);
        }

        private void RequestChunkUpdate()
        {
            int updatedChunks = 0;
            while(queuedChunks.Count > 0 && updatedChunks < MAX_CHUNKS_PER_UPDATE)
            {
                Vector3i chunk = queuedChunks.Dequeue();

                // Skip chunks that are already rendered
                if (renderedChunks.Contains(chunk))
                    continue;

                if (cachedChunks.ContainsKey(chunk))
                {
                    renderedChunks.Add(chunk);
                }
                else
                    CreateChunk(chunk);

                updatedChunks++;
            }
        }

        private void CalculateNeededChunks()
        {
            queuedChunks.Clear();
            List<Vector3i> tempChunks = [];

            for(int dx = -RENDER_DISTANCE; dx <= RENDER_DISTANCE; dx++)
            {
                for(int dz = -RENDER_DISTANCE; dz <= RENDER_DISTANCE; dz++)
                {
                    Vector3i coordinate = viewerChunk + new Vector3i(dx, 0, dz);

                    float cx = (coordinate.X + 0.5f) * TerrainGenerator.CHUNK_SIZE;
                    float cz = (coordinate.Z + 0.5f) * TerrainGenerator.CHUNK_SIZE;

                    Vector3 center = new Vector3(cx, 0f, cz);

                    float r = RENDER_DISTANCE * TerrainGenerator.CHUNK_SIZE;
                    Vector3 viewerXZ = new Vector3(viewPosition.X, 0f, viewPosition.Z);

                    if ((center - viewerXZ).LengthSquared > (r * r))
                        continue;

                    tempChunks.Add(coordinate);
                }
            }

            /*
            for(int i = 0; i < renderedChunks.Count; i++)
            {
                if (!tempChunks.Contains(renderedChunks[i]))
                    cachedChunks[renderedChunks[i]].IsActive = false;
            }
            */

            renderedChunks.RemoveAll(c => !tempChunks.Contains(c));
            BuildChunkQueue(tempChunks);
        }

        private void BuildChunkQueue(List<Vector3i> temp)
        {
            // Sort chunk queue by distance to viewer so closer chunks are rendered first
            temp.Sort((a, b) =>
            {
                Vector3 ac = TerrainGenerator.ChunkToWorldCoordinate(a) + 0.5f * TerrainGenerator.CHUNK_SIZE * Vector3.One;
                Vector3 bc = TerrainGenerator.ChunkToWorldCoordinate(b) + 0.5f * TerrainGenerator.CHUNK_SIZE * Vector3.One;

                float distanceA = (ac - viewPosition).LengthSquared;
                float distanceB = (bc - viewPosition).LengthSquared;

                return distanceA.CompareTo(distanceB);
            });

            for(int i = 0; i < temp.Count; i++)
            {
                queuedChunks.Enqueue(temp[i]);
            }
        }
    }
}
